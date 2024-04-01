using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsInventory : InventoryController
{
    [SerializeField] private WeaponBase _startingWeapon;

    protected override void Awake()
    {
        base.Awake();
        AddItem(new Weapon(_startingWeapon));
    }

    /// <summary>
    /// Adds a weapon to the inventory. If it is at max capacity, replaces the current weapon for the new one
    /// </summary>
    /// <param name="newItem">Item to add</param>
    public override void AddItem(Item newItem)
    {
        if (_items.Count < _maxItems)
        {
            base.AddItem(newItem); 
            return;
        }
        else
        {
            int weaponIndex = _items.FindIndex(a => a == _playerController.ActiveWeapon.Weapon);
            RemoveItem(weaponIndex);
            AddItemAtIndex(newItem, weaponIndex);
        }
    }    
}
