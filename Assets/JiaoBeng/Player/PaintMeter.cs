using UnityEngine;
using UnityEngine.UI;

public class PaintMeter : MonoBehaviour
{
    [Header("颜料池")]
    public float maxPaint = 200f;
    public float regenRate = 30f;
    public float regenDelay = 0.3f;

    [Header("UI")]
    public Image paintBar;

    public float CurrentPaint { get; private set; }
    public float Percent => CurrentPaint / maxPaint;

    private float _regenTimer;

    // 初始化颜料池+找UI
    private void Awake()
    {
        CurrentPaint = maxPaint;
        if (paintBar == null) paintBar = GetComponentInChildren<Image>();
    }

    // 注册回调解锁
    private void OnEnable()  => PaintManager.OnBeforeSpawn += TryConsume;
    // 取消回调解锁
    private void OnDisable() => PaintManager.OnBeforeSpawn -= TryConsume;

    // 回复颜料+更新UI
    private void Update()
    {
        _regenTimer -= Time.deltaTime;
        if (_regenTimer <= 0f && CurrentPaint < maxPaint)
            CurrentPaint = Mathf.Min(maxPaint, CurrentPaint + regenRate * Time.deltaTime);

        if (paintBar != null)
        {
            paintBar.type = Image.Type.Filled;
            paintBar.fillMethod = Image.FillMethod.Horizontal;
            paintBar.fillAmount = Percent;
        }
    }

    // 预估动作颜料是否够
    public bool HasEnough(int drops, float paintChance)
    {
        float c = paintChance > 0 ? paintChance : 1f;
        return CurrentPaint >= drops * c;
    }

    // 每滴消耗回调
    private bool TryConsume(float paintChance)
    {
        float cost = paintChance > 0 ? paintChance : 1f;
        if (CurrentPaint < cost) return false;
        CurrentPaint -= cost;
        _regenTimer = regenDelay;
        return true;
    }
}
