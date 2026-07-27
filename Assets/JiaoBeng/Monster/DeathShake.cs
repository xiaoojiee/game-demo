using UnityEngine;

public class DeathShake : MonoBehaviour
{
    [Header("抖动参数")]
    [Tooltip("抖动强度（偏移量）")]
    public float intensity = 0.15f;
    [Tooltip("抖动持续时间")]
    public float duration = 0.3f;
    [Tooltip("抖动频率（越高越快）")]
    public float frequency = 30f;

    private Healh _health;
    private Vector3 _originalPos;
    private bool _shaking;
    private float _shakeTimer;

    private void Awake()
    {
        _health = GetComponent<Healh>();
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.OnDeath += OnDeathHandler;
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDeath -= OnDeathHandler;
    }

    // 启动抖动
    private void OnDeathHandler(Transform killer)
    {
        _originalPos = transform.localPosition;
        _shaking = true;

        _shakeTimer = _health.destroyDelay > 0 ? _health.destroyDelay : duration;
    }

    // 抖动计算
    private void Update()
    {
        if (!_shaking) return;

        _shakeTimer -= Time.deltaTime;
        if (_shakeTimer <= 0f)
        {
            _shaking = false;
            transform.localPosition = _originalPos;
            return;
        }

        float t = (duration - _shakeTimer) * frequency;
        float x = (Mathf.PerlinNoise(t, 0f) - 0.5f) * 2f * intensity;
        float y = (Mathf.PerlinNoise(0f, t) - 0.5f) * 2f * intensity;

        transform.localPosition = _originalPos + new Vector3(x, y, 0);
    }
}
