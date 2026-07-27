using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPjo : MonoBehaviour
{
    private Transform Core;
    private float R;
    private float angleOffset;

    public void Init(Transform bossCore, float r, float offsetRad)
    {
        Core = bossCore;
        R = r;
        angleOffset = offsetRad;
    }

    public void UpdatePosition(float totalAngleRad)
    {
        if (Core == null) return;
        float finalAngle = totalAngleRad + angleOffset;
        float x = Core.position.x + Mathf.Cos(finalAngle) * R;
        float y = Core.position.y + Mathf.Sin(finalAngle) * R;
        transform.position = new Vector3(x, y, 0);
    }
}