using UnityEngine;

public struct Damage
{
    public int damageAmount;
    public float penetration;
    public Vector3 knockbackForce;
    public Transform damageSource;
    public float shakeIntensity;
    public float shakeDuration;
}

public interface IDamage
{
    void TakeDamage(Damage damage);
}
