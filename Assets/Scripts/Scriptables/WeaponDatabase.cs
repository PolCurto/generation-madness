using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponDatabase : ScriptableObject
{
    public WeaponBase[] weapons;

    public WeaponBase GetWeapon(int weaponID)
    {
        foreach (WeaponBase weapon in weapons)
        {
            if (weapon != null && weapon.id == weaponID) return weapon;
        }
        return null;
    }
}