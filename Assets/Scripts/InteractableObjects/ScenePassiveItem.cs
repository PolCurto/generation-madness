using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePassiveItem : InteractableObject
{
    [SerializeField] private ItemBase _itemBase;
    //protected Item _item;

    private void Awake()
    {
        //_item = new Item(_itemBase);
    }

    protected override void Interact()
    {
        Debug.Log("Interact");
        base.Interact();
        //_playerController.AddItem(_item);
        _playerController.ModifyStats(_itemBase);
        Destroy(gameObject);
    }
}
