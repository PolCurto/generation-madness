using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePassiveItem : InteractableObject
{
    [SerializeField] private ItemBase _itemBase;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _minimapIcon;
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

    public void SetBaseItem(ItemBase itemBase)
    {
        _itemBase = itemBase;
        _spriteRenderer.sprite = itemBase.itemSprite;
        _minimapIcon.sprite = itemBase.itemSprite;
    }
}