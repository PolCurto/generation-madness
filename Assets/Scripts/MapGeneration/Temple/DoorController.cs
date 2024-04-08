using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private float _roomMovingOffset;

    private Animator _animator;
    private DoorController _linkedDoor;

    private bool _open;

    public Bond Bond { get; set; }

    private void Awake()
    {
        _open = true;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //Debug.Log("Door at position: " + transform.position + "has parent connection at position: " + Bond.Connection.Position);
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
        rb.MovePosition(Bond.LinkedBond.DoorController.transform.position + (new Vector3(Bond.Direction.x, Bond.Direction.y) * _roomMovingOffset));
        Invoke(nameof(PlayerEnterRoom), .2f);
    }

    public void LinkDoor(DoorController linkedDoor)
    {
        _linkedDoor = linkedDoor;
    }

    private void PlayerEnterRoom()
    {
        TempleLevelController.Instance.OnPlayerEnterRoom(Bond.LinkedConnection.ParentRoom);
    }

    public void CloseDoor()
    {
        _animator.Play("Door_Close");
        _open = false;
        _collider.enabled = true;
    }

    public void OpenDoor()
    {
        _animator.Play("Door_Open");
        _open = true;
        _collider.enabled = false;
    }
}
