using UnityEngine;

public class Shape_Line : PaintShapeBase
{
    // 窄线喷洒
    public override void Execute(Vector3 position, Vector2 direction, int count, Color color, Vector2 speedRange, PaintSpreadSettings spreadSettings)
    {
        for (int i = 0; i < count; i++)
        {
            float angleOffset = Random.Range(-5f, 5f) * Mathf.Deg2Rad;
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