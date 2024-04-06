using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public WeaponBase WeaponBase { get; private set; }

    public int TotalBullets { get; private set; }
    public int ClipBullets { get; private set; }

    public Weapon(WeaponBase weaponBase)
    {
        WeaponBase = weaponBase;
        TotalBullets = weaponBase.maxBullets;
        ClipBullets = weaponBase.clipSize;
    }

    public void Shoot()
    {
        ClipBullets--;
    }

    public void Reload()
    {
        Debug.Log("Start reload. Total bullets: " + TotalBullets + " Clip bullets: " + ClipBullets);
        int desiredBullets = WeaponBase.clipSize - ClipBullets;
        int obtainedBullets = Mathf.Min(desiredBullets, TotalBullets);

        ClipBullets += obtainedBullets;
        TotalBullets -= obtainedBullets;

        if (TotalBullets < 0) TotalBullets = 0;
        Debug.Log("End reload. Total bullets: " + TotalBullets + " Clip bullets: " + ClipBullets);

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
