using UnityEngine;

public class Shape_QuarterCircle : PaintShapeBase
{
    [Header("扇形张开角度")]
    [Range(30f, 180f)] public float sectorAngle = 90f;

    public override void Execute(Vector3 position, Vector2 direction, int count, Color color, Vector2 speedRange, PaintSpreadSettings spreadSettings)
    {
        float halfAngle = sectorAngle * 0.5f * Mathf.Deg2Rad;
        for (int i = 0; i < count; i++)
        {
            float randomAngle = Random.Range(-halfAngle, halfAngle);
            Vector2 randomDir = new Vector2(
                Mathf.Cos(randomAngle) * direction.x - Mathf.Sin(randomAngle) * direction.y,
                Mathf.Sin(randomAngle) * direction.x + Mathf.Cos(randomAngle) * direction.y
            );
            float speed = Random.Range(speedRange.x, speedRange.y);
            PaintManager.Instance.SpawnPaintDrop(position, randomDir * speed, color, spreadSettings);
        }
    }
}