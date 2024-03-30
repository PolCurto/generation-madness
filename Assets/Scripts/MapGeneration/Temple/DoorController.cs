using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private DoorController _linkedDoor;
    private bool _open;

    public Bond Bond { get; set; }

    private void Awake()
    {
        _open = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_open) return;

        if (collision.CompareTag("Player"))
        {
            //collision.gameObject.GetComponent<Rigidbody2D>().MovePosition(_linkedDoor.transform.position);
            _linkedDoor.gameObject.SetActive(false);
            collision.transform.position = _linkedDoor.transform.position;
        }
    }

    public void LinkDoor(DoorController linkedDoor)
    {
        _linkedDoor = linkedDoor;
    }

    public void CloseDoor()
    {
        _open = false;
    }
}
