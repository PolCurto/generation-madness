using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Global Variables
    [SerializeField] private Rigidbody2D _playerRb;

    [Header("Camera Settings")]
    [SerializeField] private float _dampingX;
    [SerializeField] private float _dampingY;
    #endregion

    #region Unity Methods
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
            //HandleLookahead();
            //LookDown();
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
        Vector3 desiredPosition = _playerRb.position;
        desiredPosition.z = transform.position.z;

        //Lerps to the desired position and applies it to the camera transform
        float smoothedPositionX = Mathf.Lerp(transform.position.x, desiredPosition.x, _dampingX * Time.deltaTime);
        float smoothedPositionY = Mathf.Lerp(transform.position.y, desiredPosition.y, _dampingY * Time.deltaTime);

        Vector3 smoothedPosition = new(smoothedPositionX, smoothedPositionY, transform.position.z);
        transform.position = smoothedPosition;
    }

    /*
    /// <summary>
    /// Calculates the LookAhead offset
    /// </summary>
    private void HandleLookahead()
    {
        float playerVelocity = Mathf.Abs(_playerRb.velocity.x) * _velocitySmoother;
        float lookAheadOffset = PlayerInputsManager.Instance.ReadHorizontalInput() != 0 ? Mathf.Min(playerVelocity, _lookAheadMaxDistance) : Mathf.Max(playerVelocity, _lookAheadMinDistance);

        //Set its value to minimum 1 in case it manages to get below in certain situations
        if (lookAheadOffset < 1) lookAheadOffset = 1;
        _lookAheadDesiredDistance = _player.right.x == 1 ? lookAheadOffset : -lookAheadOffset;
    }
    */
    
    /*
    private void LookDown()
    {
        if (_playerController.IsGrounded)
        {
            _dampingY = _minDampingY;
            _verticalOffset = 3;
        }
        else if (_playerRb.velocity.y < _fallSpeedThreshold)
        {
            _dampingY = -_playerRb.velocity.y / 4;
            _verticalOffset = 0;
        }
    }
    */
}

