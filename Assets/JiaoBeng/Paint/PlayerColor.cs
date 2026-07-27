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

    // 按1/2切换颜色
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int nextIndex = CurrentColorIndex + 1;
            if (nextIndex >= colorPresets.Length)
            {
                nextIndex = 0;
            }
            // 切换颜色索引+广播事件
            SetColorByIndex(nextIndex);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            int prevIndex = CurrentColorIndex - 1;
            if (prevIndex < 0)
            {
                prevIndex = colorPresets.Length - 1;
            }
            // 切换颜色索引+广播事件
            SetColorByIndex(prevIndex);
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