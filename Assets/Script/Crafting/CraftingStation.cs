using UnityEngine;

/// <summary>
/// 合成台 — 3个输入槽 + 1个产出槽。配方驱动，槽位用统一的拿起放下逻辑。
/// </summary>
public class CraftingStation : MonoBehaviour
{
    public GameObject panelRoot;

    [Header("合成槽")]
    public CraftSlot bladeSlot, handleSlot, guardSlot;

    [Header("产出槽")]
    public OutputSlot outputSlot;

    [Header("配方")]
    public CraftingRecipe recipe;

    private WeaponStorage storage;
    private WeaponAssembler assembler;

    void Start()
    {
        storage = FindObjectOfType<WeaponStorage>();
        assembler = FindObjectOfType<WeaponAssembler>();
        outputSlot?.Refresh();
    }

    public void OnSlotChanged() => outputSlot?.Refresh();

    public bool AllSlotsFilled() =>
        bladeSlot.matCount >= bladeSlot.requiredAmount &&
        handleSlot.matCount >= handleSlot.requiredAmount &&
        guardSlot.matCount >= guardSlot.requiredAmount;

    /// <summary>锻造——产出槽点一下触发</summary>
    public void Forge()
    {
        if (recipe == null || !AllSlotsFilled()) return;

        var bm = bladeSlot.material;
        var hm = handleSlot.material;
        var gm = guardSlot.material;

        if (!recipe.Matches(bm, hm, gm)) return;
        if (!bladeSlot.ConsumeRequired(bladeSlot.requiredAmount)) return;
        if (!handleSlot.ConsumeRequired(handleSlot.requiredAmount)) return;
        if (!guardSlot.ConsumeRequired(guardSlot.requiredAmount)) return;

        var go = assembler.AssembleAndReturn(
            bm.bladePrefab, bm.bladeSprite,
            hm.handlePrefab, hm.handleSprite,
            gm.guardPrefab, gm.guardSprite);
        go.SetActive(false);

        storage.weapons.Add(go);
        storage.weaponIcons.Add(recipe.GetResultIcon(bm, hm, gm));
        int idx = storage.weapons.Count - 1;
        ReturnRemaining();
        outputSlot.Refresh();
        SlotDragHandler.ForceCarryWeapon(idx, storage);  // 直接到鼠标上
    }

    void ReturnRemaining()
    {
        var slots = new[] { bladeSlot, handleSlot, guardSlot };
        foreach (var s in slots)
            if (!s.IsEmpty()) { var (mat, cnt) = s.TakeAll(); storage.AddMaterial(mat, cnt); }
    }

    public Sprite GetPreviewSprite() => recipe != null && AllSlotsFilled() && recipe.Matches(bladeSlot.material, handleSlot.material, guardSlot.material) ? recipe.GetResultIcon(bladeSlot.material, handleSlot.material, guardSlot.material) : null;
}
