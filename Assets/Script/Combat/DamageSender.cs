using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageSender : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 10;
    public float penetration = 1f;
    public float knockbackForce = 5f;

    [Header("Feedback")]
    public float shakeIntensity = 0.2f;
    public float shakeDuration = 0.1f;

    [Header("Source")]
    public Transform damageSource;

    private Collider col;
    private TrailRenderer[] trails;

    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
        col.enabled = false;
        trails = GetComponentsInChildren<TrailRenderer>();
        if (damageSource == null)
            damageSource = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void EnableCollider()
    {
        col.enabled = true;
        foreach (var t in trails) { t.Clear(); t.enabled = true; t.emitting = true; }
    }

    public void DisableCollider()
    {
        col.enabled = false;
        foreach (var t in trails) t.emitting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponentInParent<IDamage>();
        if (target == null) return;

        var dir = (other.transform.position - transform.position).normalized;
        dir.y = 0f;

        target.TakeDamage(new Damage
        {
            damageAmount = damage,
            penetration = penetration,
            knockbackForce = dir * knockbackForce,
            damageSource = damageSource,
            shakeIntensity = shakeIntensity,
            shakeDuration = shakeDuration
        });
    }
}
