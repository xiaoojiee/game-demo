using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 产出槽 — 只出不进。点一下拿起武器（和背包格子一样逻辑）。
/// </summary>
public class OutputSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image frame, icon;
    private int storedWeaponIdx = -1;
    private WeaponStorage storage;
    private CraftingStation station;
    private Color normalFrame = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    private Color hoverFrame  = new Color(0.35f, 0.35f, 0.35f, 0.9f);

    void Start()
    {
        storage = FindObjectOfType<WeaponStorage>();
        station = GetComponentInParent<CraftingStation>();
        if (frame != null) frame.color = normalFrame;
        Refresh();
    }

    public bool IsEmpty() => storedWeaponIdx < 0;

    /// <summary>锻造完成时调用——武器放产出槽</summary>
    public void SetWeapon(int weaponIdx)
    {
        storedWeaponIdx = weaponIdx;
        Refresh();
    }

    public void Refresh()
    {
        if (icon != null)
        {
            bool has = storedWeaponIdx >= 0 && storage != null && storedWeaponIdx < storage.weaponIcons.Count;
            // 材料齐且还没武器 → 半透明预览
            if (!has && station != null && station.AllSlotsFilled())
            {
                icon.sprite = station.GetPreviewSprite();
                icon.color = new Color(1, 1, 1, 0.5f);
            }
            else if (has)
            {
                icon.sprite = storage.weaponIcons[storedWeaponIdx];
                icon.color = Color.white;
            }
            else
            {
                icon.sprite = null;
                icon.color = Color.clear;
            }
        }
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (station == null || storage == null) return;

        if (!IsEmpty())
        {
            // 拿起武器（和背包格子一样）
            SlotDragHandler.ForceCarryWeapon(storedWeaponIdx, storage);
            storedWeaponIdx = -1;
            Refresh();
            station.OnSlotChanged();
            return;
        }

        // 空的 + 材料齐 → 锻造
        if (station.AllSlotsFilled())
        {
            station.Forge();
        }
    }

    public void OnPointerEnter(PointerEventData e) { if (frame != null) frame.color = hoverFrame; }
    public void OnPointerExit(PointerEventData e)  { if (frame != null) frame.color = normalFrame; }
}
