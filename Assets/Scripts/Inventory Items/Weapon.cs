using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public WeaponBase WeaponBase { get; private set; }

    public int TotalBullets { get; set; }
    public int ClipBullets { get; set; }

    public Weapon(WeaponBase weaponBase)
    {
        WeaponBase = weaponBase;
        TotalBullets = weaponBase.maxBullets;
        ClipBullets = weaponBase.clipSize;
    }

    public Weapon(WeaponBase weaponBase, int totalBullets, int clipBullets) : this(weaponBase)
    {
        TotalBullets = totalBullets;
        ClipBullets = clipBullets;
    }

    public void Shoot()
    {
        ClipBullets--;
    }

    public void Reload()
    {
        int desiredBullets = WeaponBase.clipSize - ClipBullets;
        int obtainedBullets = Mathf.Min(desiredBullets, TotalBullets);

        ClipBullets += obtainedBullets;
        TotalBullets -= obtainedBullets;

        if (TotalBullets < 0) TotalBullets = 0;

    }

    public void RestoreAmmo(int amount)
    {
        TotalBullets += amount;
        if (TotalBullets > WeaponBase.maxBullets)
        {
            TotalBullets = WeaponBase.maxBullets;
        }
    }

}
