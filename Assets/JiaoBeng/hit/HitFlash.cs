using UnityEngine;

[RequireComponent(typeof(Healh), typeof(SpriteRenderer))]
public class HitFlash : MonoBehaviour
{
    [Header("闪烁参数")]
    public int flashCount = 3;
    public float flashInterval = 0.08f;
    [Range(0f, 1f)] public float maxStrength = 0.8f;
    public Color fallbackColor = Color.red;

    private Healh _health;
    private Material _mat;

    private bool _flashing;
    private int _flashIndex;
    private float _flashTimer;
    private bool _flashOn;

    private static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");
    private static readonly int FlashStrengthID = Shader.PropertyToID("_FlashStrength");

    private void Awake()
    {
        _health = GetComponent<Healh>();
        _mat = Instantiate(GetComponent<SpriteRenderer>().material);
        GetComponent<SpriteRenderer>().material = _mat;
    }

    private void OnEnable() => _health.OnHurt += OnHurtHandler;
    private void OnDisable() => _health.OnHurt -= OnHurtHandler;

    // 启动闪烁
    private void OnHurtHandler(Damage damage)
    {
        Color flashColor = damage.PaintColor.a > 0.01f ? damage.PaintColor : fallbackColor;

        _mat.SetColor(FlashColorID, flashColor);

        _flashing = true;
        _flashIndex = 0;
        _flashTimer = flashInterval;
        _flashOn = true;
        _mat.SetFloat(FlashStrengthID, maxStrength);
    }

    // 闪烁计时
    private void Update()
    {
        if (!_flashing) return;

        _flashTimer -= Time.deltaTime;
        if (_flashTimer <= 0f)
        {
            _flashOn = !_flashOn;
            _mat.SetFloat(FlashStrengthID, _flashOn ? maxStrength : 0f);

            if (!_flashOn)
                _flashIndex++;

            if (_flashIndex >= flashCount)
            {
                _flashing = false;
                _mat.SetFloat(FlashStrengthID, 0f);
                return;
            }

            _flashTimer = flashInterval;
        }
    }

    private void OnDestroy() => Destroy(_mat);
}
