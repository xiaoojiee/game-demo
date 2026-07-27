using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintDrop : MonoBehaviour
{
    public float gravity = 9.8f;
    public float lifetime = 2f;
    private Vector2 velocity;
    private Color paintColor;
    private SpriteRenderer sr;
    private float age;
    private bool _canPaint;

    private PaintSpreadSettings _spreadSettings;

    public Sprite[] animationSprites;
    public LayerMask layer;
    public LayerMask backgroundLayer;
    public float checkRadius = 0.2f;

    [Header("随机染色与大小")]
    [Tooltip("成功染色的概率（0~1）。1=全部染色，0=全部不染")]
    [Range(0f, 1f)] public float paintChance = 1f;
    [Tooltip("颜料滴最小缩放")]
    public float sizeMin = 0.3f;
    [Tooltip("颜料滴最大缩放")]
    public float sizeMax = 1.2f;

    // 初始化速度/颜色/大小
    public void Init(Vector2 initialVelocity, Color color, PaintSpreadSettings settings = default)
    {
        velocity = initialVelocity;
        paintColor = color;
        _spreadSettings = settings;

        float chance = settings.paintChance > 0 ? settings.paintChance : paintChance;
        float min = settings.sizeMin > 0 ? settings.sizeMin : sizeMin;
        float max = settings.sizeMax > 0 ? settings.sizeMax : sizeMax;

        _canPaint = Random.value < chance;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            transform.localScale = Vector3.one * Random.Range(min, max);
            transform.position += new Vector3(0, 0.3f, 0);

            if (animationSprites.Length > 0)
                sr.sprite = animationSprites[Random.Range(0, animationSprites.Length)];
        }
    }

    // 飞行+碰撞染色+销毁
    private void Update()
    {
        age += Time.deltaTime;
        if (age >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        velocity.y -= gravity * Time.deltaTime;
        float moveDist = velocity.magnitude * Time.deltaTime;

        if (velocity.magnitude > 0)
        {
            RaycastHit2D[] bgHits = Physics2D.CircleCastAll(transform.position, checkRadius, velocity.normalized, moveDist, backgroundLayer);
            foreach (var bgHit in bgHits)
            {
                if (bgHit.collider != null && _canPaint)
                {
                    PaintManager.Instance.SpawnSplat(bgHit.collider, bgHit.point, paintColor, _spreadSettings.wallIntensity);
                }
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity.normalized, moveDist, layer);
        if (hit.collider != null)
        {
            if (_canPaint)
                PaintManager.Instance.SpawnSplat(hit.point, hit.normal, paintColor, _spreadSettings);
            Destroy(gameObject);
            return;
        }

        transform.position += (Vector3)velocity * Time.deltaTime;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (sr != null)
        {
            float alpha = 1f - (age / lifetime);
            Color c = paintColor;
            c.a *= alpha;
            sr.color = c;
        }

    }
}