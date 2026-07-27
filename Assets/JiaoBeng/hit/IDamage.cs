using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Damage
{
    public int damageAmount;
    public Vector2 knockbackForce;
    public Transform damageSource;
    public Color PaintColor;
}
public interface IDamage
{
    void TakeDamage(Damage damage);
}
