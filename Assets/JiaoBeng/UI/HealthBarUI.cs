using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("血量条 Image（ImageType 必须是 Filled）")]
    public Image fillImage;

    [Header("测试用（Inspector 拖滑块）")]
    [Range(0f, 1f)]
    public float testFill = 1f;

    private float _targetFill = 1f;
    private float _smoothFill = 1f;
    private Healh _health;

    private void Start()
    {
        if (fillImage == null)
            fillImage = GetComponent<Image>();

        if (fillImage != null)
        {
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = 0;
        }
    }

    // 平滑过渡+测试滑块
    private void Update()
    {
        if (!Mathf.Approximately(testFill, _targetFill))
        {
            // 设血量0~1
            SetFill(testFill);
        }

        _smoothFill = Mathf.Lerp(_smoothFill, _targetFill, Time.deltaTime * 5f);
        if (fillImage != null)
            fillImage.fillAmount = _smoothFill;
    }

    // 设血量0~1
    public void SetFill(float value)
    {
        _targetFill = Mathf.Clamp01(value);
    }

    // 绑定Healh自动更新
    public void Bind(Healh health)
    {
        _health = health;
    }

    // 自动同步血量
    private void LateUpdate()
    {
        if (_health != null && _health.maxHp > 0)
        {
            SetFill((float)_health.CurrentHp / _health.maxHp);
        }
    }
}
