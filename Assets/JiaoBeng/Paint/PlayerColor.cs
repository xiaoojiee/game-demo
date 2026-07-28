using System;
using UnityEngine;

public class PlayerColor : Singleton<PlayerColor>
{
    [Header("颜色预设组")]
    public Color[] colorPresets = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta
    };

    public int CurrentColorIndex { get; private set; }

    public Color CurrentColor => colorPresets[CurrentColorIndex];

    public event Action<Color> OnColorChanged;

    // 初始颜色索引=0
    protected override void Awake()
    {
        base.Awake();
        CurrentColorIndex = 0;
    }

    // 滚轮切换颜色
    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            int dir = scroll > 0 ? 1 : -1;
            int nextIndex = CurrentColorIndex + dir;
            if (nextIndex >= colorPresets.Length) nextIndex = 0;
            if (nextIndex < 0) nextIndex = colorPresets.Length - 1;
            SetColorByIndex(nextIndex);
        }
    }

    // 切换颜色索引+广播事件
    public void SetColorByIndex(int index)
    {
        if (index < 0 || index >= colorPresets.Length)
        {
            return;
        }
        if (CurrentColorIndex == index) return;

        CurrentColorIndex = index;
        OnColorChanged?.Invoke(CurrentColor);
    }
}