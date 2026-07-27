using UnityEngine;

public enum TriggerMode { Manual, OnEnable, OnDestroy }
public enum ColorSource { Fixed, PlayerColor, FromDamage }

public class PaintSpawner : MonoBehaviour
{
    [Header("形状与数量")]
    public PaintShapeBase shape;
    public int dropCount = 25;
    public Vector2 speedRange = new Vector2(6f, 12f);

    [Header("生成点（不拖就用自身位置）")]
    public Transform spawnPoint;

    [Header("方向")]
    public Vector2 baseDirection = Vector2.up;
    public bool useCharacterFacing = false;
    [Range(-180f, 180f)] public float directionOffsetAngle = 0f;

    [Header("扩散")]
    public PaintSpreadSettings spreadSettings;

    [Header("颜料滴控制")]
    [Range(0f, 1f)] public float paintChance = 1f;
    public float minSize = 0.3f;
    public float maxSize = 1.2f;

    [Header("触发方式")]
    public TriggerMode triggerMode = TriggerMode.Manual;

    [Tooltip("玩家颜料池限制（怪物设为 false）")]
    public bool limitByPaintMeter = true;

    [Header("颜色")]
    public ColorSource colorSource = ColorSource.Fixed;
    public Color fixedColor = Color.red;

    private Healh _health;

    private void Awake()
    {
        if (shape == null) shape = GetComponent<PaintShapeBase>();
        if (colorSource == ColorSource.FromDamage)
            _health = GetComponent<Healh>();
    }

    private void OnEnable()
    {
        if (colorSource == ColorSource.FromDamage && _health != null)
            _health.OnHurt += OnHurtHandler;

        if (triggerMode == TriggerMode.OnEnable)
            Launch();
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnHurt -= OnHurtHandler;
    }

    private void OnDestroy()
    {
        if (triggerMode == TriggerMode.OnDestroy)
            Launch();
    }

    private void OnHurtHandler(Damage damage)
    {
        Launch(damage.PaintColor.a > 0.01f ? damage.PaintColor : fixedColor);
    }

    public void Launch() => Launch(GetColor());
    public void Launch(Color color)
    {
        if (shape == null || PaintManager.Instance == null) return;

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;

        Vector2 baseDir = baseDirection;
        if (useCharacterFacing)
        {
            float facing = Mathf.Sign(transform.root.localScale.x);
            baseDir.x *= facing;
        }

        float rad = directionOffsetAngle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(
            Mathf.Cos(rad) * baseDir.x - Mathf.Sin(rad) * baseDir.y,
            Mathf.Sin(rad) * baseDir.x + Mathf.Cos(rad) * baseDir.y
        );

        var s = spreadSettings;
        s.paintChance = paintChance;
        s.sizeMin = minSize;
        s.sizeMax = maxSize;
        s.limitByPaintMeter = limitByPaintMeter;
        shape.Execute(pos, dir, dropCount, color, speedRange, s);
    }

    public Color CurrentColor => GetColor();

    private Color GetColor()
    {
        switch (colorSource)
        {
            case ColorSource.Fixed:       return fixedColor;
            case ColorSource.PlayerColor: return PlayerColor.Instance != null ? PlayerColor.Instance.CurrentColor : fixedColor;
            case ColorSource.FromDamage:   return fixedColor;
            default:                      return fixedColor;
        }
    }
}
