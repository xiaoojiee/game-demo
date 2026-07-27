using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    [Header("击退参数")]
    public bool onlyHorizontal = true;
    public float damping = 1f;

    [Header("受击停顿参数")]
    [Tooltip("停顿持续时间（秒）")]
    public float hitStopDuration = 0.05f;
    [Tooltip("停顿期间是否暂停动画")]
    public bool pauseAnimation = true;
    [Tooltip("停顿期间是否冻结刚体速度")]
    public bool stopRigidbody = true;

    private Rigidbody2D _rb;
    private Healh _health;
    private Animator _animator;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Healh>();
        _animator = GetComponent<Animator>();
        if (_rb != null) _rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnHurt += OnHurtHandler;
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnHurt -= OnHurtHandler;
    }

    // 停顿后击退
    private void OnHurtHandler(Damage damage)
    {
        StopAllCoroutines();
        StartCoroutine(HitStopThenKnockback(damage));
    }

    private IEnumerator HitStopThenKnockback(Damage damage)
    {
        float originalAnimSpeed = _animator ? _animator.speed : 1f;
        Vector2 originalVelocity = _rb ? _rb.velocity : Vector2.zero;

        if (pauseAnimation && _animator != null)
            _animator.speed = 0f;

        if (stopRigidbody && _rb != null)
            _rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(hitStopDuration);

        if (pauseAnimation && _animator != null)
            _animator.speed = originalAnimSpeed;

        if (damage.damageSource == null || !enabled) yield break;
        // 施加击退力
        ApplyKnockback(damage.damageSource, damage.knockbackForce.magnitude);
    }

    // 施加击退力
    public void ApplyKnockback(Transform sourceTransform, float force)
    {
        Vector2 knockbackDir = (transform.position - sourceTransform.position).normalized;

        if (onlyHorizontal)
        {
            knockbackDir.y = 0;
            knockbackDir = knockbackDir.normalized;
        }

        _rb.velocity = Vector2.zero;
        _rb.AddForce(knockbackDir * force / damping, ForceMode2D.Impulse);
    }
}