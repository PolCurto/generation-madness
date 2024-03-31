using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private ObjectType type;

    public enum ObjectType { Weapon }
    protected PlayerController _playerController;

    protected virtual void Interact() 
    {
        _playerController.DesiredInteraction = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController component))
        {
            _playerController = component;
            //_playerController.CanInteract(true);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        //_playerController.CanInteract(false);
        _playerController = null;
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (_playerController != null && _playerController.DesiredInteraction)
        {
            Interact();
        }
    }

}