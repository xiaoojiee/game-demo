using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePaint : MonoBehaviour
{
    public PaintShapeBase shape;
    public int dropCount = 20;
    public Vector2 speedRange = new Vector2(4f, 10f);
    public float spawnYOffset = 0.3f;
    public PaintSpreadSettings spreadSettings;
    public Color fallbackColor= Color.white;
    public bool useDamageDirection=false;
    private Healh HP;
    private void Awake()
    {
        HP = GetComponent<Healh>();
        if (shape == null)
        {
            shape=GetComponent<PaintShapeBase>();
        }
    }
    private void OnEnable()
    {
        HP.OnHurt += OnHurtHandler;
    }
    private void OnDisable()
    {
        if (HP != null)
        {
            HP.OnHurt -= OnHurtHandler;
        }
    }
    // 受伤泼颜料
    private void OnHurtHandler(Damage damage)
    {
        if (shape == null) return;
        Vector3 spawnPos=transform.position+new Vector3(0,spawnYOffset,0);
        Color finalColor = damage.PaintColor.a > 0 ? damage.PaintColor : fallbackColor;
        Vector2 burstDir = Vector2.up;
        if (useDamageDirection && damage.damageSource != null)
        {
            burstDir=(transform.position-damage.damageSource.transform.position).normalized;

        }
        shape.Execute(spawnPos, burstDir, dropCount, finalColor, speedRange, spreadSettings);
    }
}
