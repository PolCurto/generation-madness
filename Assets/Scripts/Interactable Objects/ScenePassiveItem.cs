using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePassiveItem : InteractableObject
{
    [SerializeField] private ItemBase _itemBase;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _minimapIcon;
    [SerializeField] private SpriteRenderer _border;

    private Animator _animator;

    private void Awake()
    {
        if (_itemBase != null)
        {
            SetBaseItem(_itemBase);
        }
        _animator = GetComponent<Animator>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Player"))
        {
            _border.enabled = true;
            _animator.Play("Item_Interactable");
        }

    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (collision.CompareTag("Player"))
        {
            _border.enabled = false;
            _animator.SetTrigger("Stop");
        }

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
       // _border.sprite = itemBase.itemSprite;
    }
}