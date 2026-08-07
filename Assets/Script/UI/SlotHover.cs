using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 悬停高亮。挂载在每个格子上，自动读取 Image 颜色做基准。
/// </summary>
public class SlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float brightnessBoost = 0.25f;

    private Image img;
    private Color baseColor;

    void Start()
    {
        img = GetComponent<Image>();
        if (img != null) baseColor = img.color;
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (img != null) img.color = baseColor + new Color(brightnessBoost, brightnessBoost, brightnessBoost, 0);
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (img != null) img.color = baseColor;
    }
}
