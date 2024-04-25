using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int floorDepth;
    public int runType;

    public int currentMaxLife;
    public int currentLife;
    public float damageMultiplier;
    public float attackSpeed;
    public float reloadSpeed;
    public float bulletSpeed;

    public List<int> clipBullets;
    public List<int> totalBullets;
    public List<int> weaponId;

    public GameData() 
    {
        floorDepth = 0;
        runType = 0;
        currentMaxLife = 3;
        currentLife = 3;
        damageMultiplier = 1;
        attackSpeed = 1;
        reloadSpeed = 1;
        bulletSpeed = 1;

        clipBullets = new List<int>
        {
            20
        };

        totalBullets = new List<int>
        {
            30
        };

        weaponId = new List<int>
        {
            1
        };
    }
}
