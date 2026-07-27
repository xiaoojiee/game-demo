using UnityEngine;

public class PaintLauncherColorBinder : MonoBehaviour
{
    private AttackPaintSpawner _launcher;

    private void Awake()
    {
        _launcher = GetComponent<AttackPaintSpawner>();
    }

    // 订阅颜色变更事件
    private void OnEnable()
    {
        if (PlayerColor.Instance != null)
        {
            PlayerColor.Instance.OnColorChanged += UpdatePaintColor;
            // 同步颜色到泼洒器
            UpdatePaintColor(PlayerColor.Instance.CurrentColor);
        }
    }

    // 取消订阅
    private void OnDisable()
    {
        if (PlayerColor.Instance != null)
        {
            PlayerColor.Instance.OnColorChanged -= UpdatePaintColor;
        }
    }

    // 同步颜色到泼洒器
    private void UpdatePaintColor(Color newColor)
    {
        if (_launcher != null)
        {
            _launcher.paintColor = newColor;
            
        }
    }
}