using UnityEngine;

/// <summary>
/// 武器组装器 — 挂载在武器空对象上。
/// 拖入剑刃/剑柄/护手预制体，运行时自动拼好。
/// </summary>
public class WeaponAssembler : MonoBehaviour
{
    [Header("部件预制体")]
    public GameObject bladePrefab;
    public GameObject handlePrefab;
    public GameObject guardPrefab;

    [Header("部件图标")]
    public Sprite bladeIcon;
    public Sprite handleIcon;
    public Sprite guardIcon;

    [Header("部件偏移")]
    public Vector3 bladeOffset  = Vector3.zero;
    public Vector3 handleOffset = Vector3.zero;
    public Vector3 guardOffset  = Vector3.zero;

    void Start()
    {
        Assemble();
    }

    /// <summary>组装武器</summary>
    [ContextMenu("Assemble")]
    public void Assemble()
    {
        if (bladePrefab == null || handlePrefab == null || guardPrefab == null)
            return;

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        Instantiate(bladePrefab,  transform).transform.localPosition = bladeOffset;
        Instantiate(handlePrefab, transform).transform.localPosition = handleOffset;
        Instantiate(guardPrefab,  transform).transform.localPosition = guardOffset;

        var storage = FindObjectOfType<WeaponStorage>();
        if (storage != null)
            storage.AddWeapon(gameObject, bladeIcon, handleIcon, guardIcon);
    }

    /// <summary>用任意部件预制体拼一把新武器（不依赖自身配置）</summary>
    public GameObject AssembleAndReturn(
        GameObject bladePrefab, Sprite bladeSprite,
        GameObject handlePrefab, Sprite handleSprite,
        GameObject guardPrefab, Sprite guardSprite)
    {
        GameObject root = new GameObject("Weapon_Crafted");
        root.AddComponent<WeaponItem>();
        if (bladePrefab  != null) { var go = Instantiate(bladePrefab,  root.transform); go.transform.localPosition = bladeOffset; }
        if (handlePrefab != null) { var go = Instantiate(handlePrefab, root.transform); go.transform.localPosition = handleOffset; }
        if (guardPrefab  != null) { var go = Instantiate(guardPrefab,  root.transform); go.transform.localPosition = guardOffset; }
        return root;
    }
}
