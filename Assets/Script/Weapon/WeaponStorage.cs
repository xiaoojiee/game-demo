using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStorage : MonoBehaviour
{
    [Header("挂点")]
    public Transform handSlot;
    [Header("UI 容器")]
    public Transform bagGrid, hotbarParent;

    [NonSerialized] public List<GameObject> weapons = new();
    [NonSerialized] public List<Sprite> weaponIcons = new();
    [NonSerialized] public int[] bagIndex, hotbarIndex;
    [NonSerialized] public MaterialData[] bagMaterials, hotbarMaterials;
    [NonSerialized] public int[] bagMaterialCounts, hotbarMaterialCounts;
    public const int MATERIAL_SLOT = -2, MAX_STACK = 100;
    [NonSerialized] public int currentHotbar = -1;

    private PlayerCombat combat;
    private SlotDragHandler[] bagSlots, hotbarSlots;
    private GameObject handItemObj;

    void Awake()
    {
        weapons.Clear(); weaponIcons.Clear();
        bagSlots    = bagGrid      != null ? bagGrid.GetComponentsInChildren<SlotDragHandler>()      : new SlotDragHandler[0];
        hotbarSlots = hotbarParent != null ? hotbarParent.GetComponentsInChildren<SlotDragHandler>() : new SlotDragHandler[0];
        bagIndex    = new int[bagSlots.Length];      for (int i = 0; i < bagIndex.Length; i++)    bagIndex[i]    = -1;
        hotbarIndex = new int[hotbarSlots.Length];   for (int i = 0; i < hotbarIndex.Length; i++) hotbarIndex[i] = -1;
        bagMaterials      = new MaterialData[Mathf.Max(bagSlots.Length, 1)];
        bagMaterialCounts = new int[bagMaterials.Length];
        hotbarMaterials      = new MaterialData[Mathf.Max(hotbarSlots.Length, 1)];
        hotbarMaterialCounts = new int[hotbarMaterials.Length];
        combat = FindObjectOfType<PlayerCombat>();
        RefreshAll();
    }

    void Update()
    {
        float s = Input.GetAxis("Mouse ScrollWheel");
        if (s > 0) ScrollHotbar(-1); else if (s < 0) ScrollHotbar(1);
        for (int i = 0; i < Mathf.Min(hotbarSlots.Length, 8); i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) EquipFromHotbar(i);
    }

    // ==================== 查询 ====================

    bool GetSlotArrays(SlotDragHandler.SlotType t, out int[] idx, out MaterialData[] mats, out int[] cnts)
    {
        idx = t == SlotDragHandler.SlotType.Bag ? bagIndex : hotbarIndex;
        mats = t == SlotDragHandler.SlotType.Bag ? bagMaterials : hotbarMaterials;
        cnts = t == SlotDragHandler.SlotType.Bag ? bagMaterialCounts : hotbarMaterialCounts;
        return idx != null && mats != null;
    }

    public bool HasWeapon(SlotDragHandler.SlotType t, int s) { GetSlotArrays(t, out var idx, out _, out _); return s>=0&&s<idx.Length&&idx[s]>=0; }
    public bool HasMaterial(SlotDragHandler.SlotType t, int s) { GetSlotArrays(t, out var idx, out var mats, out _); return s>=0&&s<idx.Length&&idx[s]==MATERIAL_SLOT&&mats[s]!=null; }
    public MaterialData GetSlotMaterial(SlotDragHandler.SlotType t, int s) { GetSlotArrays(t, out var idx, out var mats, out _); return (s>=0&&s<idx.Length&&idx[s]==MATERIAL_SLOT) ? mats[s] : null; }
    public bool IsSelected(SlotDragHandler.SlotType t, int s) => t == SlotDragHandler.SlotType.Hotbar && s == currentHotbar;

    public Sprite GetIcon(SlotDragHandler.SlotType t, int s)
    {
        if (HasMaterial(t, s)) { GetSlotArrays(t, out _, out var m, out _); return m[s]?.icon; }
        GetSlotArrays(t, out var idx, out _, out _);
        int wi = (s>=0&&s<idx.Length) ? idx[s] : -1;
        return (wi>=0 && wi<weaponIcons.Count) ? weaponIcons[wi] : null;
    }
    public int GetMaterialCountAt(SlotDragHandler.SlotType t, int s, bool countBagOnly = false) { GetSlotArrays(t, out var idx, out _, out var c); return s>=0&&s<idx.Length&&idx[s]==MATERIAL_SLOT?c[s]:0; }
    public Sprite GetIconRaw(int wi) => (wi>=0&&wi<weaponIcons.Count) ? weaponIcons[wi] : null;

    // ==================== 材料存取 ====================

    public bool AddMaterial(MaterialData mat, int count = 1)
    {
        if (mat==null||count<=0||bagIndex==null) return false;
        for (int i = 0; i < bagIndex.Length; i++)
        {
            if (bagIndex[i]==MATERIAL_SLOT && bagMaterials[i]==mat)
            {
                int add = Mathf.Min(count, MAX_STACK - bagMaterialCounts[i]);
                bagMaterialCounts[i] += add;
                if (count > add) AddMaterial(mat, count - add);
                RefreshAll(); return true;
            }
        }
        for (int i = 0; i < bagIndex.Length; i++)
        {
            if (bagIndex[i]==-1) { bagIndex[i]=MATERIAL_SLOT; bagMaterials[i]=mat; bagMaterialCounts[i]=Mathf.Min(count,MAX_STACK); RefreshAll(); return true; }
        }
        return false;
    }

    public int GetMaterialCount(MaterialData mat) { int t=0; for(int i=0;i<bagIndex.Length;i++) if(bagIndex[i]==MATERIAL_SLOT&&bagMaterials[i]==mat) t+=bagMaterialCounts[i]; return t; }
    public bool ConsumeMaterial(MaterialData mat, int count)
    {
        for (int i = 0; i < bagIndex.Length; i++)
        {
            if (bagIndex[i]==MATERIAL_SLOT && bagMaterials[i]==mat)
            {
                if (bagMaterialCounts[i] < count) return false;
                bagMaterialCounts[i] -= count;
                if (bagMaterialCounts[i] <= 0) { bagIndex[i] = -1; bagMaterials[i] = null; }
                RefreshAll(); return true;
            }
        }
        return false;
    }

    public int TakeMaterial(int bagSlotIdx, out MaterialData mat) { return TakeMaterial(bagSlotIdx, out mat, out _); }
    public int TakeMaterial(int bagSlotIdx, out MaterialData mat, out int count)
    {
        if (bagSlotIdx<0||bagSlotIdx>=bagIndex.Length||bagIndex[bagSlotIdx]!=MATERIAL_SLOT) { mat=null; count=0; return 0; }
        mat=bagMaterials[bagSlotIdx]; count=bagMaterialCounts[bagSlotIdx];
        bagIndex[bagSlotIdx]=-1; bagMaterials[bagSlotIdx]=null; RefreshAll(); return count;
    }

    public void PutMaterial(MaterialData mat, int count, SlotDragHandler.SlotType t, int s)
    {
        GetSlotArrays(t, out var idx, out var mats, out var cnts);
        if (mat==null||count<=0||s<0||s>=idx.Length) return;
        if (idx[s]==MATERIAL_SLOT&&mats[s]==mat) cnts[s]=Mathf.Min(cnts[s]+count, MAX_STACK);
        else { idx[s]=MATERIAL_SLOT; mats[s]=mat; cnts[s]=Mathf.Min(count,MAX_STACK); }
        RefreshAll();
    }

    // ==================== 武器存取 ====================

    public int TakeWeapon(SlotDragHandler.SlotType t, int s)
    {
        GetSlotArrays(t, out var idx, out _, out _);
        if (s<0||s>=idx.Length) return -1;
        int wi=idx[s]; idx[s]=-1;
        if (t==SlotDragHandler.SlotType.Hotbar && s==currentHotbar && wi>=0) { if(wi<weapons.Count){weapons[wi].SetActive(false);weapons[wi].transform.SetParent(transform);} combat?.EquipWeapon(null); }
        RefreshAll(); return wi;
    }
    public void PutWeapon(int wi, SlotDragHandler.SlotType t, int s)
    {
        GetSlotArrays(t, out var idx, out _, out _);
        if (s<0||s>=idx.Length) return; idx[s]=wi;
        if (t==SlotDragHandler.SlotType.Hotbar&&s==currentHotbar) EquipFromHotbar(s);
        RefreshAll();
    }
    public void DestroyWeapon(int wi)
    {
        if(wi<0||wi>=weapons.Count) return;
        for(int i=0;i<bagIndex.Length;i++) if(bagIndex[i]==wi) bagIndex[i]=-1;
        for(int i=0;i<hotbarIndex.Length;i++) if(hotbarIndex[i]==wi) hotbarIndex[i]=-1;
        if(wi<weapons.Count&&weapons[wi]!=null){weapons[wi].SetActive(false);Destroy(weapons[wi]);}
        weapons.RemoveAt(wi); weaponIcons.RemoveAt(wi); RefreshAll();
    }
    public void AddWeapon(GameObject weapon, Sprite bladeIcon = null, Sprite handleIcon = null, Sprite guardIcon = null)
    {
        weapon.SetActive(false); weapon.transform.SetParent(transform);
        weapons.Add(weapon); weaponIcons.Add(SpriteCombiner.Combine(bladeIcon, handleIcon, guardIcon));
        int newIdx=weapons.Count-1, placedH=-1;
        foreach(var arr in new[]{hotbarIndex, bagIndex})
            for(int i=0;i<arr.Length;i++)
                if(arr[i]==-1){arr[i]=newIdx;placedH=i;break;}
        if(placedH>=0) EquipFromHotbar(placedH);
        RefreshAll();
    }

    // ==================== 装备 ====================

    public void EquipFromHotbar(int s)
    {
        if(s<0||s>=hotbarIndex.Length) return;
        UnequipCurrent();
        int wi=hotbarIndex[s];

        if(wi==MATERIAL_SLOT && hotbarMaterials[s]!=null)
        {
            var m=hotbarMaterials[s];
            if(m.heldPrefab!=null){handItemObj=Instantiate(m.heldPrefab,handSlot);handItemObj.transform.localPosition=Vector3.zero;handItemObj.transform.localRotation=Quaternion.identity;handItemObj.transform.localScale=Vector3.one;}
            combat?.EquipWeapon(null);
        }
        else if(wi>=0 && wi<weapons.Count)
        {
            var w=weapons[wi];
            w.transform.SetParent(handSlot);w.transform.localPosition=Vector3.zero;w.transform.localRotation=Quaternion.identity;w.transform.localScale=Vector3.one;
            w.SetActive(true);
            combat?.EquipWeapon(w);
        }
        currentHotbar=s; RefreshAll();
    }
    void UnequipCurrent()
    {
        if(currentHotbar<0||currentHotbar>=hotbarIndex.Length) return;
        int wi=hotbarIndex[currentHotbar];
        if(wi>=0&&wi<weapons.Count&&weapons[wi]!=null){weapons[wi].SetActive(false);weapons[wi].transform.SetParent(transform);}
        if(handItemObj!=null){Destroy(handItemObj);handItemObj=null;}
    }
    void ScrollHotbar(int d){int l=hotbarSlots.Length;if(l==0)return;EquipFromHotbar((currentHotbar+d+l)%l);}

    public void RefreshAll() { foreach(var s in bagSlots) if(s!=null) s.Refresh(); foreach(var s in hotbarSlots) if(s!=null) s.Refresh(); }
}
