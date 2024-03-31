using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] protected int _maxItems;
    [SerializeField] protected PlayerController _playerController;

    protected List<InventoryItem> _items;

    protected virtual void Awake()
    {
        _items = new List<InventoryItem>();
    }

    /// <summary>
    /// Adds the item to the inventory
    /// </summary>
    /// <param name="newItem">Item to add</param>
    public virtual void AddItem(InventoryItem newItem)
    {
        _items.Add(newItem);
    }

    /// <summary>
    /// Adds the item to the inventory at the given index
    /// </summary>
    /// <param name="newItem">Item to add</param>
    /// <param name="index">List index</param>
    public virtual void AddItemAtIndex(InventoryItem newItem, int index)
    {
        _items.Insert(index, newItem);
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    /// <param name="item">Item to remove</param>
    public virtual void RemoveItem(InventoryItem item)
    {
        _items.Remove(item);
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    /// <param name="index">Index to remove from</param>
    public virtual void RemoveItem(int index)
    {
        _items.RemoveAt(index);
    }

    /// <summary>
    /// Gets the item from the list at the given index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public InventoryItem GetItem(int index) 
    {
        return _items[index];
    }
}
