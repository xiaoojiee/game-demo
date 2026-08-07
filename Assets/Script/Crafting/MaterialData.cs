using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Material", fileName = "Material_Iron")]
public class MaterialData : ScriptableObject
{
    public string materialName = "Iron";
    public Sprite icon;
    public GameObject heldPrefab;

    [Header("Costs")]
    public int bladeCost  = 2;
    public int handleCost = 1;
    public int guardCost  = 1;

    [Header("Part Prefabs")]
    public GameObject bladePrefab;
    public GameObject handlePrefab;
    public GameObject guardPrefab;
    public Sprite bladeSprite;
    public Sprite handleSprite;
    public Sprite guardSprite;
}
