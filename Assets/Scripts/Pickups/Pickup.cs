using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController player))
        {
            OnPickUp(player);
        }
    }

    protected virtual void OnPickUp(PlayerController player)
    {
        Destroy(gameObject);
    }
}
