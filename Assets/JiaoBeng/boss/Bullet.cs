using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    [Header("基础配置")]
    [Tooltip("弹幕最大飞行时间（秒），超时自动生成石头")]
    public float maxLifeTime = 3f;
    [Tooltip("生成的石头预制体（需挂载Block组件和碰撞体）")]
    public GameObject stonePrefab;
    [Tooltip("目标Tilemap，用于网格对齐计算")]
    public Tilemap targetTilemap;

    private Vector2 moveDirection;
    private float moveSpeed;
    private float lifeTimer;
    private bool hasTriggered;
    private Collider2D bulletCollider;

    private void Awake()
    {
        bulletCollider = GetComponent<Collider2D>();
    }

    public void Init(Vector2 direction, float speed, GameObject blockPrefab, Tilemap tilemap)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        stonePrefab = blockPrefab;
        targetTilemap = tilemap;
        lifeTimer = 0f;
        hasTriggered = false;
        if (bulletCollider != null) bulletCollider.enabled = true;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        if (hasTriggered) return;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLifeTime)
        {
            OnLifeTimeEnd();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        Tilemap hitTilemap = other.GetComponent<Tilemap>();
        if (hitTilemap != null)
        {
            MarkTriggered();

            Vector2 hitSurfacePoint = other.ClosestPoint(transform.position);
            Vector2 spawnBasePos = hitSurfacePoint - moveDirection * 0.1f;

            SpawnAlignedStone(spawnBasePos, hitTilemap);
            DestroyBullet();
            return;
        }

        Block hitStone = other.GetComponent<Block>();
        if (hitStone != null)
        {
            MarkTriggered();
            StackStoneByHitFace(hitStone, other);
            DestroyBullet();
        }
    }

    private void MarkTriggered()
    {
        hasTriggered = true;
        if (bulletCollider != null) bulletCollider.enabled = false;
    }

    private void OnLifeTimeEnd()
    {
        if (hasTriggered) return;
        MarkTriggered();

        if (stonePrefab != null && targetTilemap != null)
        {
            SpawnAlignedStone(transform.position, targetTilemap);
        }
        DestroyBullet();
    }

    private void StackStoneByHitFace(Block hitStone, Collider2D stoneCollider)
    {
        if (stonePrefab == null || targetTilemap == null) return;

        Vector2 hitPoint = stoneCollider.ClosestPoint(transform.position);
        Vector3Int stoneCell = targetTilemap.WorldToCell(hitStone.transform.position);
        Vector3 cellSize = targetTilemap.cellSize;

        Vector2 offset = hitPoint - (Vector2)hitStone.transform.position;
        Vector3Int stackOffset = new Vector3Int(0, 1, 0);

        if (Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
        {
            stackOffset = offset.x > 0 ? new Vector3Int(1, 0, 0) : new Vector3Int(-1, 0, 0);
        }
        else
        {
            stackOffset = offset.y > 0 ? new Vector3Int(0, 1, 0) : new Vector3Int(0, -1, 0);
        }

        Vector3Int targetCell = stoneCell + stackOffset;
        Vector3 spawnPos = targetTilemap.CellToWorld(targetCell) + new Vector3(cellSize.x * 0.5f, cellSize.y * 0.5f, 0);
        spawnPos.z = 0;

        if (IsCellHasBlock(spawnPos, cellSize)) return;

        Instantiate(stonePrefab, spawnPos, Quaternion.identity);
    }

    private void SpawnAlignedStone(Vector3 worldPos, Tilemap usedTilemap)
    {
        if (stonePrefab == null || usedTilemap == null) return;

        Vector3Int cellPos = usedTilemap.WorldToCell(worldPos);
        Vector3 cellSize = usedTilemap.cellSize;
        Vector3 alignedPos = usedTilemap.CellToWorld(cellPos) + new Vector3(cellSize.x * 0.5f, cellSize.y * 0.5f, 0);
        alignedPos.z = 0;

        if (IsCellHasBlock(alignedPos, cellSize)) return;

        Instantiate(stonePrefab, alignedPos, Quaternion.identity);
    }

    private bool IsCellHasBlock(Vector3 center, Vector3 cellSize)
    {
        Vector2 checkSize = new Vector2(cellSize.x * 0.8f, cellSize.y * 0.8f);
        Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f);
        return hit != null && hit.GetComponent<Block>() != null;
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}