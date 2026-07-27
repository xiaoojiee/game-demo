using UnityEngine;

public class ScaleByHealth : MonoBehaviour
{
    [Header("体型映射")]
    [Tooltip("最大血量（对应 scale=2）")]
    public float maxHp = 100f;
    [Tooltip("最小血量（对应 scale=0.2）")]
    public float minHp = 10f;

    private Healh _health;
    private Vector3 _baseScale;

    private void Awake()
    {
        _health = GetComponent<Healh>();
        _baseScale = transform.localScale;
    }

    private void Start()
    {
        // 血量→体型映射
        UpdateScale();
    }

    // 血量→体型映射
    private void UpdateScale()
    {
        if (_health == null) return;

        float hp = _health.CurrentHp;

        float t = Mathf.InverseLerp(minHp, maxHp, hp);
        float scale = Mathf.Lerp(0.2f, 2f, t);

        transform.localScale = _baseScale * scale;
    }
}
