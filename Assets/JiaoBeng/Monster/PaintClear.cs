using UnityEngine;
using UnityEngine.Tilemaps;

public class PaintClear : MonoBehaviour
{
    [Header("默认参数（普通落地）")]
    [Range(0.1f, 0.9f)]
    public float decayRate = 0.5f;
    [Range(0.005f, 0.5f)]
    public float minIntensity = 0.02f;

    [Header("射线检测")]
    [Tooltip("只检测这个层的碰撞体")]
    public LayerMask groundLayer;
    [Tooltip("向下射线的最大距离")]
    public float rayDistance = 2f;

    // 消除脚下颜料(指数衰减环形)
    public void ClearPaintAt(Vector3 worldPos,
        float? overrideDecay = null,
        float? overrideMin = null)
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.down, rayDistance, groundLayer);

        if (hit.collider == null)
        {
            return;
        }

        Tilemap tilemap = hit.collider.GetComponent<Tilemap>();
        if (tilemap == null)
            tilemap = hit.collider.GetComponentInParent<Tilemap>();

        if (tilemap == null)
        {
            return;
        }

        float x = overrideDecay ?? decayRate;
        float y = overrideMin ?? minIntensity;

        Vector3Int centerCell = tilemap.WorldToCell(hit.point);

        int maxR = Mathf.FloorToInt(Mathf.Log(y) / Mathf.Log(x));
        int clearedCount = 0;

        for (int ring = 0; ring <= maxR; ring++)
        {
            float intensity = Mathf.Pow(x, ring);
            if (intensity < y) break;

            for (int dx = -ring; dx <= ring; dx++)
            {
                for (int dy = -ring; dy <= ring; dy++)
                {
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (Mathf.RoundToInt(dist) != ring) continue;
                    intensity=1;
                    Vector3Int cell = centerCell + new Vector3Int(dx, dy, 0);
                    if (ClearTile(tilemap, cell, intensity))
                        clearedCount++;
                }
            }
        }
    }

    // 单格Lerp到白色
    private bool ClearTile(Tilemap tilemap, Vector3Int cell, float intensity)
    {
        if (!tilemap.HasTile(cell)) return false;
        Color current = tilemap.GetColor(cell);
        Color cleared = Color.Lerp(current, Color.white, intensity);
        tilemap.SetTileFlags(cell, TileFlags.None);
        tilemap.SetColor(cell, cleared);
        return true;
    }
}
