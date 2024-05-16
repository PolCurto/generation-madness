using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Global Variables
    public static CameraController Instance;

    [SerializeField] private Rigidbody2D _playerRb;
    [SerializeField] private PlayerController _playerController;

    [Header("Camera Settings")]
    [Range(0,1)]
    [SerializeField] private float _mouseOffset;
    [SerializeField] private float _maxOffset;
    [SerializeField] private float _dampingX;
    [SerializeField] private float _dampingY;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
        ResetOffset();
    }

    private void Start()
    {
        Vector3 newPositon = transform.position;
        newPositon.z = transform.position.z;
        transform.position = newPositon;

        if (_playerRb == null)
        {
            Debug.LogError("[CameraController] La referència a Jugador és null");
        }
    }

    void LateUpdate()
    {
        if (_playerRb != null)
        {
            FollowPlayer();
        }
    }
    #endregion

    /// <summary>
    /// Follows the player position smoothly
    /// </summary>
    private void FollowPlayer()
    {
        //Gets the desired position
        Vector2 AB = _playerController.MousePosition - _playerRb.position;
        Vector2 offset = Vector2.zero;

        if (AB.magnitude > 5)
        {
            offset = AB * MouseOffset;
            offset = new Vector2(Mathf.Clamp(offset.x, -MaxOffset, MaxOffset), Mathf.Clamp(offset.y, -MaxOffset, MaxOffset));

            if (Mathf.Abs(AB.x * MouseOffset) > MaxOffset)
            {
                AB.x = MaxOffset;
            }
            if (Mathf.Abs(AB.y) > MaxOffset)
            {
                AB.y = MaxOffset;
            }
        }

        Vector3 desiredPosition = _playerRb.position + offset;
        desiredPosition.z = transform.position.z;

        //Lerps to the desired position and applies it to the camera transform
        float smoothedPositionX = Mathf.Lerp(transform.position.x, desiredPosition.x, _dampingX * Time.deltaTime);
        float smoothedPositionY = Mathf.Lerp(transform.position.y, desiredPosition.y, _dampingY * Time.deltaTime);

        Vector3 smoothedPosition = new(smoothedPositionX, smoothedPositionY, transform.position.z);
        transform.position = smoothedPosition;
    }

    /// <summary>
    /// Resets the offset of the camera to the default value
    /// </summary>
    public void ResetOffset()
    {
        MaxOffset = _maxOffset;
        MouseOffset = _mouseOffset;
    }

    public float MaxOffset { get; set; }
    public float MouseOffset { get; set; }
}

