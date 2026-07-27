using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExplosionHandle : MonoBehaviour
{
    [Header("基础配置")]
    [Tooltip("目标瓦片地图，由发射脚本自动传入，预制体无需手动赋值")]
    public Tilemap targetTilemap;

    [Tooltip("爆炸后生成的方块预制体")]
    public GameObject Block;

    [Tooltip("透明伤害判定体预制体")]
    public GameObject DamageBlock;

    [Tooltip("爆炸造成的伤害值")]
    public int Damage;

    [Tooltip("伤害判定体存在时长（秒），值越小越不容易重复判定")]
    public float Life;
    [Tooltip("方块生成延迟时间（秒），0=立刻生成")]
    public float blockSpawnDelay = 0.15f;

    [Header("功能开关")]
    [Tooltip("开启后会清除爆炸范围内Tilemap上的原生瓦片")]
    public bool ClearOriginalTile = false;

    [Tooltip("开启后只在空位置生成方块，避免堆叠")]
    public bool onlySpawnOnEmpty = true;

    [Tooltip("方块所在的图层，用于空位检测")]
    public LayerMask blockLayer;

    [Header("爆炸阻挡设置")]
    [Tooltip("开启后爆炸会被阻挡物挡住，无法穿墙")]
    public bool enableBlocker = false;

    [Tooltip("阻挡物所在的图层（不可破坏的墙、地形等）")]
    public LayerMask BlockerLayer;

    private IExplosionShape explosionShape;
    private HashSet<Vector3Int> _processedGrids;
    private bool _hasExploded = false;

    private void Awake()
    {
        explosionShape = GetComponent<IExplosionShape>();
        if (explosionShape == null)
        {
        }

        _processedGrids = new HashSet<Vector3Int>();
    }

    public void SetTargetTilemap(Tilemap tilemap)
    {
        targetTilemap = tilemap;
    }

    public void ExecuteExplosion(Vector3 explosionWorldPos)
    {
        if (_hasExploded)
            return;
        _hasExploded = true;

        if (explosionShape == null || targetTilemap == null)
        {
            return;
        }

        _processedGrids.Clear();

        List<Vector3Int> affectedGrid = explosionShape.GetAffectedGridPositions(explosionWorldPos, targetTilemap);

        int targetLayerIndex = LayerMaskToIndex(blockLayer);

        foreach (Vector3Int pos in affectedGrid)
        {
            
            if (_processedGrids.Contains(pos))
                continue;
            _processedGrids.Add(pos);

            Vector3 gridCenter = targetTilemap.CellToWorld(pos) + targetTilemap.cellSize / 2;
            gridCenter.z = 0;

            if (enableBlocker)
            {
                Vector2 direction = gridCenter - explosionWorldPos;
                float distance = direction.magnitude;
                RaycastHit2D hit = Physics2D.Raycast(
                    explosionWorldPos,
                    direction.normalized,
                    distance,
                    BlockerLayer
                );

                if (hit.collider != null)
                    continue;
            }

            if (ClearOriginalTile)
                targetTilemap.SetTile(pos, null);

            if (DamageBlock != null)
            {
                GameObject zone = Instantiate(DamageBlock, gridCenter, Quaternion.identity);
                zone.transform.localScale = targetTilemap.cellSize;

                ExplosionDamage zoneScript = zone.GetComponent<ExplosionDamage>();
                if (zoneScript != null)
                {
                    zoneScript.damage = Damage;
                    zoneScript.lifeTime = Life;
                }
            }

            if (Block != null)
            {
                if (onlySpawnOnEmpty)
                {
                    Vector2 checkSize = targetTilemap.cellSize * 0.9f;
                    Collider2D existBlock = Physics2D.OverlapBox(
                        gridCenter,
                        checkSize,
                        0f,
                        blockLayer);

                    if (existBlock != null)
                        continue;
                }

                GameObject newBlock = Instantiate(Block, gridCenter, Quaternion.identity);
                if (targetLayerIndex >= 0)
                {
                    newBlock.layer = targetLayerIndex;
                }
            }
        }

        Destroy(gameObject);
    }

    private int LayerMaskToIndex(LayerMask mask)
    {
        int maskValue = mask.value;
        if (maskValue == 0)
            return -1;

        for (int i = 0; i < 32; i++)
        {
            if ((maskValue & (1 << i)) != 0)
            {
                return i;
            }
        }
        return -1;
    }
}