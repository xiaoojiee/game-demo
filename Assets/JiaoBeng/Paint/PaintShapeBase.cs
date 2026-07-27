using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PaintShapeBase : MonoBehaviour
{
    public abstract void Execute(Vector3 position, Vector2 direction, int count, Color color, Vector2 speedRange, PaintSpreadSettings spreadSettings);
}