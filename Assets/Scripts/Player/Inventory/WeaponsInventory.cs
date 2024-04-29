using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsInventory : InventoryController
{
    [SerializeField] private GameObject _pickableWeapon;
    
    [SerializeField] private WeaponDatabase _database;

    protected override void Awake()
    {
        base.Awake();
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
            int weaponIndex = GetActiveWeaponIndex();
            Weapon oldWeapon = (Weapon)_items[weaponIndex];

            RemoveItem(weaponIndex);
            AddItemAtIndex(newItem, weaponIndex);

            Instantiate(_pickableWeapon, gameObject.transform.position, Quaternion.identity).GetComponentInChildren<SceneWeapon>().SetWeapon(oldWeapon);
        }
    }
    
    public int GetActiveWeaponIndex()
    {
        return _items.FindIndex(a => a == _playerController.ActiveWeapon.Weapon);
    }

    public void AddWeapon(int id, int clipBullets, int totalBullets, int index)
    {
        Weapon weapon = new Weapon(_database.GetWeapon(id), totalBullets, clipBullets);
        AddItem(weapon);

        UIController.Instance.UpdateWeaponAtIndex(index, weapon);
    }
}
