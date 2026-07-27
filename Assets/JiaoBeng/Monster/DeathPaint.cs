using UnityEngine;

public class DeathPaint : MonoBehaviour
{
    [Header("爆射参数")]
    public PaintShapeBase shape;
    public int dropCount = 30;
    public Vector2 speedRange = new Vector2(5f, 12f);
    public Color paintColor = Color.red;
    public PaintSpreadSettings spreadSettings;

    private void Awake()
    {
        if (shape == null) shape = GetComponent<PaintShapeBase>();
    }

    // 死亡爆发颜料
    private void OnDestroy()
    {
        if (shape == null || PaintManager.Instance == null) return;

        Vector3 spawnPos = transform.position + new Vector3(0, 0.3f, 0);
        shape.Execute(spawnPos, Vector2.up, dropCount, paintColor, speedRange, spreadSettings);
    }
}
