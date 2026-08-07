using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat")]
    public GameObject currentWeapon;
    public DamageSender[] unarmedSenders;
    private DamageSender[] weaponSenders;

    void Awake()
    {
        CacheWeapon();
    }

    public void EnableWeaponDamage()
    {
        var senders = HasDamageCapable() ? weaponSenders : unarmedSenders;
        foreach (var s in senders)
            if (s != null) s.EnableCollider();
    }

    public void DisableWeaponDamage()
    {
        var senders = HasDamageCapable() ? weaponSenders : unarmedSenders;
        foreach (var s in senders)
            if (s != null) s.DisableCollider();
    }

    public bool HasDamageCapable() => currentWeapon != null && currentWeapon.activeInHierarchy
        && currentWeapon.GetComponentInChildren<DamageSender>() != null;

    public void CacheWeapon()
    {
        weaponSenders = currentWeapon != null
            ? currentWeapon.GetComponentsInChildren<DamageSender>()
            : System.Array.Empty<DamageSender>();
    }

    public void EquipWeapon(GameObject newWeapon)
    {
        currentWeapon = newWeapon;
        CacheWeapon();
    }
}
