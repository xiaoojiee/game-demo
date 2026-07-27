using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorIndicatorUI : MonoBehaviour
{
    [Header("引用")]
    public Image colorBlock;
    public TMP_Text indexText;

    private void Awake()
    {
        if (colorBlock == null)
            colorBlock = GetComponent<Image>();
        if (indexText == null)
            indexText = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        if (PlayerColor.Instance != null)
        {
            PlayerColor.Instance.OnColorChanged += UpdateUI;

            UpdateUI(PlayerColor.Instance.CurrentColor);
        }
        else
        {
        }
    }

    private void OnDisable()
    {
        if (PlayerColor.Instance != null)
        {
            PlayerColor.Instance.OnColorChanged -= UpdateUI;
        }
    }

    private void UpdateUI(Color currentColor)
    {
        if (colorBlock != null)
        {
            Color displayColor = currentColor;
            displayColor.a = 1f;
            colorBlock.color = displayColor;
        }

        if (indexText != null)
        {
            indexText.text = PlayerColor.Instance.CurrentColorIndex.ToString();
        }
    }

    private void OnDestroy()
    {
        if (PlayerColor.Instance != null)
        {
            PlayerColor.Instance.OnColorChanged -= UpdateUI;
        }
    }
}