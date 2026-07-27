using UnityEngine;

public class PaintBurstOnEnable : MonoBehaviour
{
    public PaintShapeBase shape;
    public int dropCount = 20;
    public Vector2 baseDirection = Vector2.up;
    [Tooltip("在基础方向上的旋转偏移角度")]
    public float directionOffsetAngle = 0f;
    public Vector2 speedRange = new Vector2(4f, 10f);
    public PaintSpreadSettings spreadSettings;

    // 激活时泼洒一次
    private void OnEnable()
    {
        if (shape == null || PaintManager.Instance == null) return;

        float rad = directionOffsetAngle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(
            Mathf.Cos(rad) * baseDirection.x - Mathf.Sin(rad) * baseDirection.y,
            Mathf.Sin(rad) * baseDirection.x + Mathf.Cos(rad) * baseDirection.y
        );

        var s = spreadSettings;
        s.limitByPaintMeter = true;
        Color color = PlayerColor.Instance != null
            ? PlayerColor.Instance.CurrentColor : Color.white;
        shape.Execute(transform.position, dir, dropCount,
            color, speedRange, s);
    }
}
