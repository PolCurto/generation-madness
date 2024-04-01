using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] protected int _maxItems;
    [SerializeField] protected PlayerController _playerController;

    protected List<Item> _items;

    protected virtual void Awake()
    {
        _items = new List<Item>();
    }

    /// <summary>
    /// Adds the item to the inventory
    /// </summary>
    /// <param name="newItem">Item to add</param>
    public virtual void AddItem(Item newItem)
    {
        _items.Add(newItem);
    }

    /// <summary>
    /// Adds the item to the inventory at the given index
    /// </summary>
    /// <param name="newItem">Item to add</param>
    /// <param name="index">List index</param>
    public virtual void AddItemAtIndex(Item newItem, int index)
    {
        _items.Insert(index, newItem);
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    /// <param name="item">Item to remove</param>
    public virtual void RemoveItem(Item item)
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
    public bool TryGetItem(int index, out Item item) 
    {
        item = null;

        if (index >= _items.Count) return false;

        item = _items[index];
        return true;
    }
}
