using UnityEngine;

/// <summary>
/// 材料库存 — 挂在 Player 上。材料直接进背包格子。
/// </summary>
public class MaterialInventory : MonoBehaviour
{
    [Header("调试：初始材料")]
    public MaterialData startMaterial;
    public int startCount = 10;

    void Start()
    {
        if (startMaterial != null && startCount > 0)
        {
            var ws = FindObjectOfType<WeaponStorage>();
            ws?.AddMaterial(startMaterial, startCount);
        }
    }

    public static int GetCount(MaterialData mat)
    {
        var ws = FindObjectOfType<WeaponStorage>();
        return ws != null ? ws.GetMaterialCount(mat) : 0;
    }

    public static bool Consume(MaterialData mat, int count)
    {
        var ws = FindObjectOfType<WeaponStorage>();
        return ws != null && ws.ConsumeMaterial(mat, count);
    }
}
