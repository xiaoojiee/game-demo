using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    [Header("停顿参数")]
    [Tooltip("停顿持续时间（秒）")]
    public float stopDuration = 0.05f;

    [Tooltip("是否同时暂停动画（Animator.speed = 0）")]
    public bool pauseAnimation = true;

    [Tooltip("是否同时停止刚体移动（velocity = 0）")]
    public bool stopRigidbody = true;

    private Animator anim;
    private Rigidbody2D rb;
    private Healh health;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Healh>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnHurt += OnHurtHandler;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnHurt -= OnHurtHandler;
    }

    // 执行停顿
    private void OnHurtHandler(Damage damage)
    {
        StopAllCoroutines();
        StartCoroutine(HitStopCoroutine());
    }

    private IEnumerator HitStopCoroutine()
    {
        float originalAnimSpeed = anim ? anim.speed : 1f;
        Vector2 originalVelocity = rb ? rb.velocity : Vector2.zero;

        if (pauseAnimation && anim != null)
            anim.speed = 0f;

        if (stopRigidbody && rb != null)
            rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(stopDuration);

        if (pauseAnimation && anim != null)
            anim.speed = originalAnimSpeed;

        
    }
}