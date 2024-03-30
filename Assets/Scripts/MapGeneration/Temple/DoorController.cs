using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private bool _open;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private float _roomMovingOffset;

    private DoorController _linkedDoor;

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
            GoNextRoom(collision.gameObject.GetComponent<Rigidbody2D>());
        }
    }

    /// <summary>
    /// Moves the player to the next room and closes the doors
    /// </summary>
    /// <param name="rb">Player rigidbody</param>
    private void GoNextRoom(Rigidbody2D rb)
    {
        rb.MovePosition(_linkedDoor.transform.position + (new Vector3(Bond.Direction.x, Bond.Direction.y) * _roomMovingOffset));
        
        //Invoke(nameof(CloseLinkedDoor), .2f);
    }

    public void LinkDoor(DoorController linkedDoor)
    {
        _linkedDoor = linkedDoor;
    }

    private void CloseLinkedDoor()
    {
        _linkedDoor.CloseDoor();
    }

    public void CloseDoor()
    {
        _open = false;
        _collider.enabled = true;
    }
}
