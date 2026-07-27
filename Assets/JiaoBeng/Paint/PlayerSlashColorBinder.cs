using UnityEngine;
public class PlayerSlashColorBinder : MonoBehaviour
{
    [Tooltip("目标SpriteRenderer，不填则自动获取自身组件")]
    public SpriteRenderer targetSprite;
    private Material _runtimeMat;

    // 实例化材质
    private void Awake()
    {
        if (targetSprite == null)
            targetSprite = GetComponent<SpriteRenderer>();
        if (targetSprite != null)
        {
            _runtimeMat = Instantiate(targetSprite.material);
            targetSprite.material = _runtimeMat;
        }
        else
        {
        }
    }

    // 订阅事件+初始化颜色
    private void Start()
    {
        if (PlayerColor.Instance != null)
        {
            PlayerColor.Instance.OnColorChanged += UpdateTargetColor;
            // 更新材质颜色
            UpdateTargetColor(PlayerColor.Instance.CurrentColor);
        }
        else
        {
        }
    }

    // 取消订阅
    private void OnDisable()
    {
        if (PlayerColor.Instance != null)
        {
            PlayerColor.Instance.OnColorChanged -= UpdateTargetColor;
        }
    }

    // 更新材质颜色
    private void UpdateTargetColor(Color newColor)
    {
        if (_runtimeMat != null)
        {
            _runtimeMat.SetColor("_TargetColor", newColor);
        }
    }

    private void OnDestroy()
    {
        if (_runtimeMat != null)
            Destroy(_runtimeMat);
    }
}