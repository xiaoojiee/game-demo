using System;
using UnityEngine;

/// <summary>
/// 开局背包配置 — 挂在 Player 上。运行时自动把配置的材料放进背包。
/// </summary>
public class StartingInventory : MonoBehaviour
{
    [Serializable]
    public class MaterialEntry
    {
        public MaterialData material;
        public int count = 1;
    }

    [Header("开局材料")]
    public MaterialEntry[] startMaterials;

    void Start()
    {
        var ws = FindObjectOfType<WeaponStorage>();
        if (ws == null) return;

        foreach (var e in startMaterials)
        {
            if (e.material != null && e.count > 0)
                ws.AddMaterial(e.material, e.count);
        }
    }
}
