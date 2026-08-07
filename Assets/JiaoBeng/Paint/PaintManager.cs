using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct PaintSpreadSettings
{
    [Tooltip("落点中心染色强度（基础值，建议保持1）")]
    public float baseIntensity;
    [Tooltip("墙面轨迹单点染色强度")]
    public float wallIntensity;

    [Header("扩散控制")]
    [Tooltip("最大扩散半径（格子数）：绝对边界，再多颜料也不会超出这个范围")]
    public float diffusionRadius;
    [Tooltip("每向外1格的衰减比例：比如0.7 → 中心1 → 第1格0.7 → 第2格0.49…")]
    [Range(0.1f, 0.95f)] public float decayRatio;

    [Tooltip("椭圆宽高比：1 = 正圆，大于1 = 横向椭圆，小于1 = 纵向椭圆")]
    public float aspectRatio;

    [Tooltip("是否受玩家颜料池限制（怪物设为 false）")]
    public bool limitByPaintMeter;

    [HideInInspector] public float paintChance;
    [HideInInspector] public float sizeMin;
    [HideInInspector] public float sizeMax;
}

public class PaintManager : Singleton<PaintManager>
{
    [Header("核心引用")]
    public GameObject paintDrop;
    public Tilemap groundTilemap;
    [Header("颜色与形状设置")]
    public Color paintColor = Color.white;

    [Header("全局默认扩散参数")]
    public float baseIntensity = 1f;
    public float wallIntensity = 0.08f;
    public float diffusionRadius = 4f;
    [Range(0.1f, 0.95f)] public float decayRatio = 0.7f;
    public float aspectRatio = 1f;

    [Header("强度下限")]
    [Tooltip("外圈强度低于此值就卡住，不继续衰减，避免边缘太淡")]
    [Range(0f, 0.5f)] public float intensityFloor = 0f;

    // 单例初始化+找Tilemap
    protected override void Awake()
    {
        base.Awake();
        transform.parent = null;
        RefreshGroundTilemap();
        // 场景重载时主动刷新 Tilemap 引用，避免指向已销毁对象
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshGroundTilemap();
    }

    private void RefreshGroundTilemap()
    {
        // 优先通过名字精确找 Ground Tilemap
        var allTilemaps = FindObjectsOfType<Tilemap>();
        foreach (var tm in allTilemaps)
        {
            if (tm.gameObject.name.Contains("Ground"))
            {
                groundTilemap = tm;
                return;
            }
        }
        // 找不到就叫 Ground 的，回退到第一个
        if (allTilemaps.Length > 0)
            groundTilemap = allTilemaps[0];
    }

    public static System.Func<float, bool> OnBeforeSpawn;

    // 生成颜料滴
    public void SpawnPaintDrop(Vector3 position, Vector2 vector, Color? color = null,
        PaintSpreadSettings settings = default)
    {
        if (settings.limitByPaintMeter && OnBeforeSpawn != null)
        {
            float chance = settings.paintChance > 0 ? settings.paintChance : 1f;
            if (!OnBeforeSpawn(chance))
                return;
        }

        GameObject drop = Instantiate(paintDrop, position, Quaternion.identity, transform);
        PaintDrop pd = drop.GetComponent<PaintDrop>();
        if (pd != null)
        {
            Color c = color ?? paintColor;
            pd.Init(vector, c, settings);
        }
    }

    // 地面溅射染色
    public void SpawnSplat(Vector3 pos, Vector2 normal, Color color, PaintSpreadSettings settings = default)
    {
        if (groundTilemap == null) RefreshGroundTilemap();
        if (groundTilemap == null) return;
        // 椭圆扩散+指数衰减染色
        ProcessTilemap(groundTilemap, pos, normal, color, settings);
    }

    // 地面溅射染色
    public void SpawnSplat(Collider2D col, Vector3 point, Color color, float wallIntensity = 0f)
    {
        if (col == null) return;
        Tilemap targetTilemap = col.GetComponent<Tilemap>();
        if (targetTilemap == null) targetTilemap = col.GetComponentInParent<Tilemap>();
        if (targetTilemap == null) return;
        Vector3Int startCell = targetTilemap.WorldToCell(point);
        float finalIntensity = wallIntensity > 0 ? wallIntensity : this.wallIntensity;
        // 单格叠加染色
        PaintSingleTile(targetTilemap, startCell, color, finalIntensity);
    }

    // 椭圆扩散+指数衰减染色
    private void ProcessTilemap(Tilemap tilemap, Vector3 pos, Vector2 normal, Color color, PaintSpreadSettings settings)
    {
        float finalBase = settings.baseIntensity > 0 ? settings.baseIntensity : baseIntensity;
        float finalRadius = settings.diffusionRadius > 0 ? settings.diffusionRadius : diffusionRadius;
        float finalDecay = settings.decayRatio > 0 ? settings.decayRatio : decayRatio;
        float finalAspect = settings.aspectRatio > 0 ? settings.aspectRatio : aspectRatio;

        Vector3Int centerCell = tilemap.WorldToCell(pos);

        float a = finalRadius * Mathf.Sqrt(finalAspect);
        float b = finalRadius / Mathf.Sqrt(finalAspect);

        int xMin = Mathf.FloorToInt(-a) - 1;
        int xMax = Mathf.CeilToInt(a) + 1;
        int yMin = Mathf.FloorToInt(-b) - 1;
        int yMax = Mathf.CeilToInt(b) + 1;

        for (int dx = xMin; dx <= xMax; dx++)
        {
            for (int dy = yMin; dy <= yMax; dy++)
            {
                Vector3Int targetCell = centerCell + new Vector3Int(dx, dy, 0);
                if (!tilemap.HasTile(targetCell)) continue;

                float nx = dx / a;
                float ny = dy / b;
                float normDist = Mathf.Sqrt(nx * nx + ny * ny);

                if (normDist > 1f) continue;

                float gridDistance = normDist * finalRadius;

                float intensity = finalBase * Mathf.Pow(finalDecay, gridDistance);

                if (intensityFloor > 0 && intensity < intensityFloor)
                    intensity = intensityFloor;

                // 单格叠加染色
                PaintSingleTile(tilemap, targetCell, color, intensity);
            }
        }
    }

    // 单格叠加染色
    private void PaintSingleTile(Tilemap tilemap, Vector3Int cell, Color color, float intensity)
    {
        if (tilemap == null) return;
        Color currentColor = tilemap.GetColor(cell);
        Color finalColor = Color.Lerp(currentColor, color, intensity);
        tilemap.SetTileFlags(cell, TileFlags.None);
        tilemap.SetColor(cell, finalColor);
    }

    // 单格叠加染色
    private void PaintSingleTile(Vector3Int cell, Color color, float intensity)
    {
        // 单格叠加染色
        PaintSingleTile(groundTilemap, cell, color, intensity);
    }

    // 怪物死亡爆发颜料
    public void SplashMonster(Vector3 position, int count = 20, Color? color = null)
    {
        Color c = color ?? paintColor;
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float speed = Random.Range(4f, 10f);
            // 生成颜料滴
            SpawnPaintDrop(position, dir * speed, c);
        }
    }
}