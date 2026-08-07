using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamage
{
    [Header("Stats")]
    public int maxHealth = 100;
    public float defense = 5f;
    public float deflectForce = 6f;

    private int currentHealth;
    private KnockbackReceiver knockback;

    void Start()
    {
        currentHealth = maxHealth;
        knockback = GetComponent<KnockbackReceiver>();
    }

    public void TakeDamage(Damage damage)
    {
        if (currentHealth <= 0) return;

        float ratio = damage.penetration / Mathf.Max(defense, 0.01f);
        int finalDamage = Mathf.RoundToInt(damage.damageAmount * Mathf.Clamp01(ratio));

        currentHealth -= finalDamage;
        if (finalDamage > 0) knockback?.ApplyKnockback(damage.knockbackForce);

        if (ratio < 0.5f && damage.damageSource != null)
        {
            var psm = damage.damageSource.GetComponent<PlayerStateMachine>();
            if (psm != null)
            {
                Vector3 dir = (damage.damageSource.position - transform.position).normalized;
                dir.y = 0;
                psm.ApplyKnockback(dir * deflectForce);
                psm.SwitchState(psm.idleState);
            }
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Destroy(gameObject);
        }
    }
}
