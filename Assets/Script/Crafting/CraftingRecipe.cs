using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe", fileName = "Recipe_IronSword")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName = "Iron Sword";

    [Header("Input")]
    public MaterialData bladeMaterial;
    public MaterialData handleMaterial;
    public MaterialData guardMaterial;

    [Header("Cost")]
    public int bladeCost  = 2;
    public int handleCost = 1;
    public int guardCost  = 1;

    public bool Matches(MaterialData blade, MaterialData handle, MaterialData guard)
        => blade  != null && blade.bladePrefab   != null
        && handle != null && handle.handlePrefab != null
        && guard  != null && guard.guardPrefab   != null;

    public Sprite GetResultIcon(MaterialData blade, MaterialData handle, MaterialData guard)
        => SpriteCombiner.Combine(blade.bladeSprite, handle.handleSprite, guard.guardSprite);
}
