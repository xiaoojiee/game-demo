using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healh : MonoBehaviour,IDamage
{
    public int maxHp=50;
    public float hitCooldown = 0.1f;
    public float destroyDelay=0f;
    public event Action<Damage> OnHurt;
    public event Action<Transform> OnDeath;
    private int hp;
    public int CurrentHp => hp;
   
    private bool isInCooldown;
    private void Start()
    {
        hp = maxHp;
    }
    // 扣血+触发事件
    public void TakeDamage(Damage damage)
    {
        if (isInCooldown) return;
        hp-=damage.damageAmount;
        StartCoroutine(HitCooldownCoroutine());
        OnHurt?.Invoke(damage);
        if (hp <= 0)
        {
            OnDeath?.Invoke(damage.damageSource);
            Destroy(gameObject,destroyDelay);
        }
    }
    private IEnumerator HitCooldownCoroutine()
    {
        isInCooldown=true;
        yield return new WaitForSeconds(hitCooldown);
        isInCooldown=false;
    }
}
