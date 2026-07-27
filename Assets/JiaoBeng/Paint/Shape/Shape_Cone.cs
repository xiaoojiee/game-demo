using UnityEngine;

public class Shape_Cone : PaintShapeBase
{
    [Header("锥形角度")]
    [Tooltip("最小偏转角度")]
    public float minAngle = -30f;
    [Tooltip("最大偏转角度")]
    public float maxAngle = 30f;

    // 锥形喷洒
    public override void Execute(Vector3 position, Vector2 direction, int count, Color color, Vector2 speedRange, PaintSpreadSettings spreadSettings)
    {
        if (PaintManager.Instance == null) return;

        for (int i = 0; i < count; i++)
        {
            float angleOffset = Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;
            Vector2 baseDir = direction.normalized;
            Vector2 randomDir = new Vector2(
                Mathf.Cos(angleOffset) * baseDir.x - Mathf.Sin(angleOffset) * baseDir.y,
                Mathf.Sin(angleOffset) * baseDir.x + Mathf.Cos(angleOffset) * baseDir.y
            );
            float speed = Random.Range(speedRange.x, speedRange.y);
            PaintManager.Instance.SpawnPaintDrop(position, randomDir * speed, color, spreadSettings);
        }
    }
}
