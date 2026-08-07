using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SlotDragHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public enum SlotType { Bag, Hotbar }
    public SlotType slotType;
    public int slotIndex;

    public Color normalColor   = new Color(0.25f, 0.25f, 0.25f, 1f);
    public Color hoverColor    = new Color(0.4f, 0.4f, 0.4f, 1f);
    public Color selectedColor = new Color(0.5f, 0.5f, 0.3f, 1f);

    private Image frame, icon;
    private TMP_Text countText;
    private bool isHovered;
    private WeaponStorage storage;

    // 手持状态
    private static GameObject cursorItem, cursorCountObj;
    private static TextMeshProUGUI cursorCountText;
    private static int carriedWeaponIdx = -1;
    private static WeaponStorage carriedStorage;
    private static bool carryingMat;
    private static MaterialData carriedMatData;
    private static int carriedMatCount;

    void Awake()
    {
        var imgs = GetComponentsInChildren<Image>();
        frame = imgs.Length > 0 ? imgs[0] : null;
        icon  = imgs.Length > 1 ? imgs[1] : frame;
        var ct = transform.Find("CountText");
        countText = ct != null ? ct.GetComponent<TMP_Text>() : null;
        slotIndex = transform.GetSiblingIndex();
        slotType = transform.parent.name.StartsWith("Bag") ? SlotType.Bag : SlotType.Hotbar;
    }

    void Start() { storage = Object.FindObjectOfType<WeaponStorage>(); Refresh(); }
    void Update() { if (cursorItem != null) cursorItem.transform.position = Input.mousePosition; }

    public void Refresh()
    {
        if (storage == null) return;
        bool isMat = storage.HasMaterial(slotType, slotIndex);
        bool hasWeapon = !isMat && storage.HasWeapon(slotType, slotIndex);
        bool sel = storage.IsSelected(slotType, slotIndex);

        if (icon != null)
        {
            Sprite sp = (isMat||hasWeapon) ? storage.GetIcon(slotType,slotIndex) : null;
            if (sp == null && (isMat||hasWeapon)) sp = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,4,4), Vector2.zero);
            icon.sprite = sp;
            icon.enabled = sp != null;
        }
        if (countText != null)
        {
            int cnt = isMat ? storage.GetMaterialCountAt(slotType, slotIndex) : 0;
            countText.text = cnt > 1 ? $"×{cnt}" : "";
        }
        if (frame != null) frame.color = sel ? selectedColor : isHovered ? hoverColor : normalColor;
    }

    // ========== 点击（Minecraft 风格） ==========

    public void OnPointerClick(PointerEventData e)
    {
        if (storage == null) return;
        bool right = e.button == PointerEventData.InputButton.Right;
        bool slotWpn = storage.HasWeapon(slotType, slotIndex);
        bool slotMat = storage.HasMaterial(slotType, slotIndex);
        bool handWpn = carriedWeaponIdx >= 0;
        bool handMat = carryingMat;
        bool handHas = handWpn || handMat;
        bool sameMat = handMat && slotMat && storage.GetSlotMaterial(slotType, slotIndex) == carriedMatData;

        if (!handHas)
        {
            if (slotWpn && !right)      CarryWpn(storage.TakeWeapon(slotType, slotIndex));
            else if (slotMat && !right) PickUpMat(false);
            else if (slotMat && right)  PickUpMat(true);
        }
        else if (handWpn)
        {
            if (slotWpn)      { int old = storage.TakeWeapon(slotType, slotIndex); PlaceWpn(); CarryWpn(old); }
            else if (!slotMat) PlaceWpn();
        }
        else // handMat
        {
            if (sameMat && !right) MergeHandIntoSlot(carriedMatCount);
            else if (sameMat && right) MergeHandIntoSlot(1);
            else if (slotMat)    { var (m,c) = TakeSlotMat(); PlaceMat(carriedMatCount); CarryMat(m,c); }
            else if (!slotWpn)   PlaceMat(right ? 1 : carriedMatCount);
        }
    }

    // ========== 拿起 ==========
    void PickUpMat(bool half)
    {
        int total = storage.TakeMaterial(slotIndex, out MaterialData mat);
        int take = half ? Mathf.Max(1, total/2) : total;
        if (total - take > 0) storage.PutMaterial(mat, total - take, slotType, slotIndex);
        carriedStorage = storage;
        CarryMat(mat, take);
    }
    (MaterialData, int) TakeSlotMat() { storage.TakeMaterial(slotIndex, out MaterialData m, out int c); return (m, c); }

    // ========== 放下/合并 ==========
    void PlaceWpn()  { storage.PutWeapon(carriedWeaponIdx, slotType, slotIndex); ClearCursor(); }
    void PlaceMat(int count)
    {
        int n = Mathf.Min(count, carriedMatCount);
        storage.PutMaterial(carriedMatData, n, slotType, slotIndex);
        carriedMatCount -= n;
        if (carriedMatCount <= 0) ClearCursor(); else ShowCursor();
    }
    void MergeHandIntoSlot(int count)
    {
        int before = storage.GetMaterialCountAt(slotType, slotIndex);
        storage.PutMaterial(carriedMatData, count, slotType, slotIndex);
        int added = storage.GetMaterialCountAt(slotType, slotIndex) - before;
        carriedMatCount -= added;
        if (carriedMatCount <= 0) ClearCursor(); else ShowCursor();
    }

    // ========== 手持 ==========
    void CarryWpn(int idx)  { carriedWeaponIdx = idx; carryingMat = false; carriedStorage = storage; ShowCursor(); storage.RefreshAll(); }
    static void CarryMat(MaterialData m, int c) { carryingMat = true; carriedMatData = m; carriedMatCount = c; carriedWeaponIdx = -1; ShowCursor(); }

    static void ClearCursor()
    {
        carriedWeaponIdx = -1; carryingMat = false; carriedMatData = null; carriedMatCount = 0; carriedStorage = null;
        DestroyCursor();
    }
    static void DestroyCursor() { if (cursorItem != null) { Object.Destroy(cursorItem); cursorItem = null; } if (cursorCountObj != null) { Object.Destroy(cursorCountObj); cursorCountObj = null; cursorCountText = null; } }

    // ========== 拖拽 ==========
    private static GameObject dragClone;
    public void OnBeginDrag(PointerEventData e)
    {
        if (storage == null) return;
        bool isMat = storage.HasMaterial(slotType, slotIndex);
        bool hasWpn = storage.HasWeapon(slotType, slotIndex);
        if (!isMat && !hasWpn) return;

        dragClone = new GameObject("DragClone", typeof(RectTransform), typeof(Image));
        dragClone.transform.SetParent(Object.FindObjectOfType<Canvas>().transform, false);
        dragClone.GetComponent<Image>().sprite = storage.GetIcon(slotType, slotIndex);
        dragClone.GetComponent<Image>().raycastTarget = false;
        dragClone.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
        dragClone.transform.position = e.position;
    }
    public void OnDrag(PointerEventData e)
    {
        if (dragClone != null) dragClone.transform.position = e.position;
    }
    public void OnEndDrag(PointerEventData e)
    {
        if (dragClone != null) { Destroy(dragClone); dragClone = null; }

        if (storage == null) return;
        bool isMat = storage.HasMaterial(slotType, slotIndex);

        var results = new System.Collections.Generic.List<RaycastResult>();
        GetComponentInParent<GraphicRaycaster>().Raycast(e, results);
        foreach (var r in results)
        {
            var cs = r.gameObject.GetComponentInParent<CraftSlot>();
            if (cs != null && isMat && cs.IsEmpty())
            {
                var m = storage.GetSlotMaterial(slotType, slotIndex);
                if (m != null) { cs.SetMaterial(m); storage.ConsumeMaterial(m, 1); }
                return;
            }
        }
    }
    public void OnDrop(PointerEventData e) { }

    // ========== 合成槽互通 ==========
    public static void CraftSlotClicked(CraftSlot slot, bool right)
    {
        bool handWpn = carriedWeaponIdx >= 0, handMat = carryingMat;
        if (!handWpn && !handMat)
        {
            if (slot.IsEmpty()) return;
            var (mat, count) = slot.TakeAll();
            if (right) { int half = Mathf.Max(1, count/2); slot.SetMaterial(mat, count-half); count = half; }
            carryingMat = true; carriedMatData = mat; carriedMatCount = count; ShowCursor();
        }
        else if (handMat)
        {
            if (slot.IsEmpty()) { int drop = right ? 1 : carriedMatCount; slot.SetMaterial(carriedMatData, drop); carriedMatCount -= drop; if (carriedMatCount<=0) ClearCursor(); else ShowCursor(); }
            else if (slot.material == carriedMatData) { int drop = right ? 1 : carriedMatCount; slot.AddMaterial(carriedMatData, drop); carriedMatCount -= drop; if (carriedMatCount<=0) ClearCursor(); else ShowCursor(); }
            else { var (m,c) = slot.TakeAll(); slot.SetMaterial(carriedMatData, carriedMatCount); carriedStorage = Object.FindObjectOfType<WeaponStorage>(); CarryMat(m,c); }
        }
    }
    public static void DropOntoCraftSlot(CraftSlot slot)
    {
        if (!carryingMat || carriedMatData == null) return;
        if (slot.IsEmpty()) { slot.SetMaterial(carriedMatData, carriedMatCount); ClearCursor(); }
        else if (slot.material == carriedMatData) { slot.AddMaterial(carriedMatData, carriedMatCount); ClearCursor(); }
    }

    /// <summary>外部强制拿起武器到鼠标</summary>
    public static void ForceCarryWeapon(int weaponIdx, WeaponStorage ws)
    {
        carriedWeaponIdx = weaponIdx;
        carryingMat = false;
        carriedStorage = ws;
        ShowCursor();
    }

    // ========== Cleanup ==========
    public static void TryDestroyCarried() { if (carriedWeaponIdx >= 0) carriedStorage.DestroyWeapon(carriedWeaponIdx); ClearCursor(); }
    public static void AutoStash(WeaponStorage storage)
    {
        if (carriedWeaponIdx >= 0)
        {
            foreach (var t in new[] { SlotType.Hotbar, SlotType.Bag })
            {
                var arr = t == SlotType.Bag ? storage.bagIndex : storage.hotbarIndex;
                for (int i = 0; i < arr.Length; i++) { if (arr[i] == -1) { storage.PutWeapon(carriedWeaponIdx, t, i); ClearCursor(); return; } }
            }
        }
        else if (carryingMat && carriedMatData != null) { storage.AddMaterial(carriedMatData, carriedMatCount); ClearCursor(); }
    }

    static void ShowCursor()
    {
        DestroyCursor();
        bool isWpn = carriedWeaponIdx >= 0;
        Sprite sp = isWpn ? Object.FindObjectOfType<WeaponStorage>().GetIconRaw(carriedWeaponIdx) : carriedMatData?.icon;
        int size = isWpn ? 120 : 90;
        cursorItem = new GameObject("CursorItem", typeof(RectTransform), typeof(Image));
        cursorItem.transform.SetParent(Object.FindObjectOfType<Canvas>().transform, false);
        cursorItem.GetComponent<Image>().sprite = sp;
        cursorItem.GetComponent<Image>().raycastTarget = false;
        cursorItem.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
        if (!isWpn && carriedMatCount > 1)
        {
            cursorCountObj = new GameObject("Count", typeof(RectTransform), typeof(TextMeshProUGUI));
            cursorCountObj.transform.SetParent(cursorItem.transform, false);
            cursorCountText = cursorCountObj.GetComponent<TextMeshProUGUI>();
            cursorCountText.text = $"×{carriedMatCount}"; cursorCountText.fontSize = 18; cursorCountText.color = Color.white;
            cursorCountText.alignment = TextAlignmentOptions.BottomRight; cursorCountText.raycastTarget = false;
            var rt = cursorCountObj.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(1,0); rt.pivot = new Vector2(1,0);
            rt.sizeDelta = new Vector2(60,25); rt.anchoredPosition = new Vector2(10,-5);
        }
    }

    public void OnPointerEnter(PointerEventData e) { isHovered = true; Refresh(); }
    public void OnPointerExit(PointerEventData e)  { isHovered = false; Refresh(); }
}
