using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CraftSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public string slotLabel;
    public int requiredAmount = 1;   // 需要多少材料
    public Image frame, icon;
    public TMP_Text labelText, countText;

    public MaterialData material { get; private set; }
    public int matCount { get; private set; }

    private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    private Color hoverColor  = new Color(0.35f, 0.35f, 0.35f, 0.9f);

    void Start()
    {
        if (labelText != null) labelText.text = slotLabel;
        if (frame != null) frame.color = normalColor;
        Refresh();
    }

    public void Refresh()
    {
        if (icon != null) { icon.sprite = material?.icon; icon.color = material != null ? Color.white : Color.clear; }
        if (countText != null) countText.text = material != null ? $"{matCount}/{requiredAmount}" : "";
    }

    public bool IsEmpty() => material == null;

    public void SetMaterial(MaterialData mat, int count = 1) { material = mat; matCount = count; Refresh(); FindObjectOfType<CraftingStation>()?.OnSlotChanged(); }
    public (MaterialData, int) TakeAll() { var r = (material, matCount); material = null; matCount = 0; Refresh(); FindObjectOfType<CraftingStation>()?.OnSlotChanged(); return r; }
    public bool ConsumeRequired(int required) { if (matCount < required) return false; matCount -= required; if (matCount <= 0) material = null; Refresh(); FindObjectOfType<CraftingStation>()?.OnSlotChanged(); return true; }
    public void AddMaterial(MaterialData mat, int count) { if (material == null) { material = mat; matCount = count; } else if (material == mat) matCount = Mathf.Min(matCount + count, 100); Refresh(); FindObjectOfType<CraftingStation>()?.OnSlotChanged(); }

    public void OnPointerClick(PointerEventData e) { SlotDragHandler.CraftSlotClicked(this, e.button == PointerEventData.InputButton.Right); }
    public void OnBeginDrag(PointerEventData e) { }
    public void OnDrag(PointerEventData e) { }
    public void OnEndDrag(PointerEventData e)
    {
        var results = new System.Collections.Generic.List<RaycastResult>();
        GetComponentInParent<GraphicRaycaster>().Raycast(e, results);
        foreach (var r in results)
        {
            var slot = r.gameObject.GetComponentInParent<SlotDragHandler>();
            if (slot != null && slot.slotType == SlotDragHandler.SlotType.Bag && material != null)
            {
                FindObjectOfType<WeaponStorage>().AddMaterial(material, matCount);
                material = null; matCount = 0; Refresh(); return;
            }
        }
    }
    public void OnDrop(PointerEventData e) { SlotDragHandler.DropOntoCraftSlot(this); }
    public void OnPointerEnter(PointerEventData e) { if (frame != null) frame.color = hoverColor; }
    public void OnPointerExit(PointerEventData e)  { if (frame != null) frame.color = normalColor; }
}
