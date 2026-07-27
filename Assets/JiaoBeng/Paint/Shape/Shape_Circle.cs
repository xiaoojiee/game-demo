using UnityEngine;

public class Shape_Circle : PaintShapeBase
{
    // 360°喷洒
    public override void Execute(Vector3 position, Vector2 direction, int count, Color color, Vector2 speedRange, PaintSpreadSettings spreadSettings)
    {
        if (PaintManager.Instance == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float speed = Random.Range(speedRange.x, speedRange.y);
            PaintManager.Instance.SpawnPaintDrop(position, dir * speed, color, spreadSettings);
        }
    }
}