using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSender : MonoBehaviour
{
    public int damage = 2;
    public string Tag = "Monster";
    public bool enableKnockback = true;
    public float knockbackForce = 8f;
    public Color paintColor= Color.white;
    public AttackPaintSpawner APS;
    public PaintSpawner ps;
    public CinemachineImpulseSource impulseSource;
    // 碰撞→构造Damage→调用IDamage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        APS = GetComponent<AttackPaintSpawner>();
        ps = GetComponent<PaintSpawner>();

        if (ps != null) paintColor = ps.CurrentColor;
        else if (APS != null) paintColor = APS.paintColor;
        else paintColor = Color.white;
        if (!collision.CompareTag(Tag))
        {
            return;
        }
        IDamage damageable=collision.GetComponentInParent<IDamage>();
        if (damageable == null)return;
        Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
        Damage damageData = new Damage
        {
            damageAmount = damage,
            knockbackForce = knockbackDir * knockbackForce,
            damageSource = transform,
            PaintColor= paintColor
        };

        damageable.TakeDamage(damageData);
        impulseSource.GenerateImpulse();
        
    }
}
