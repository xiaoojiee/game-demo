using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPaintSpawner : MonoBehaviour
{
    [Header("形状引用（同物体上挂的形状脚本）")]
    public PaintShapeBase _shape;
    [Header("出生点")]
    public Transform spawnPoint;
    [Header("泼洒方向")]
    public bool useCharacterFacing = true;
    public float directionOffsetAngle = 0f;

    [Header("泼洒基础参数")]
    public int dropCount = 25;
    public Color paintColor = Color.red;
    public Vector2 speedRange = new Vector2(6f, 12f);

    [Header("扩散渗透参数")]
    public PaintSpreadSettings spreadSettings;

    private void Awake()
    {
        if (_shape == null) _shape = GetComponent<PaintShapeBase>();
        
    }

    // 执行泼洒
    public void Launch()
    {
        if (_shape == null)
        {
            return;
        }

        Vector3 finalPos = spawnPoint != null
            ? spawnPoint.position
            : transform.position;

        Vector2 baseDir = useCharacterFacing
            ? new Vector2(transform.root.localScale.x, 0).normalized
            : transform.right;

        float rad = directionOffsetAngle * Mathf.Deg2Rad;
        Vector2 finalDir = new Vector2(
            Mathf.Cos(rad) * baseDir.x - Mathf.Sin(rad) * baseDir.y,
            Mathf.Sin(rad) * baseDir.x + Mathf.Cos(rad) * baseDir.y
        );

        var settings = spreadSettings;
        settings.limitByPaintMeter = true;
        _shape.Execute(finalPos, finalDir, dropCount, paintColor, speedRange, settings);
    }
}