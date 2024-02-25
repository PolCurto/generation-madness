using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Global Variables
    [SerializeField] protected Rigidbody2D _rigidbody;

    [Header("Global Stats")]
    [SerializeField] protected int _cost;
    [SerializeField] protected int _maxLife;
    [SerializeField] protected float _velocity;
    [SerializeField] protected float _detectionDistance;
    [SerializeField] protected float _enablingDistance;
    [SerializeField] protected float _attackDistance;

    protected Transform _player;
    private bool _isColiding;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _player = GameObject.Find("Player").transform;
        _pathToTake = new List<Vector2>();
    }

    protected void Start()
    {
        StartCoroutine("MoveAround");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            StopCoroutine("MoveAround");
            StartCoroutine("MoveAround");
        } 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerWithinRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerWithinRange = false;
        }
    }

    protected virtual void Update()
    {
        PlayerPosition();

        if (!_isEnabled) return;

        Debug.Log("Path nodes: " + _pathToTake.Count);
        for (int i = 0; i < _pathToTake.Count - 1; i++)
        {
            Debug.DrawLine(_pathToTake[i], _pathToTake[i + 1]);
        }
        
        DetectPlayer();
        LosePlayer();
        UpdatePath();
    }

    protected virtual void FixedUpdate()
    {
        if (!_isEnabled) return;
    }
    #endregion

    #region Player Detection
    [SerializeField] protected float _loseDetectionTime;

    protected bool _playerWithinRange;
    protected bool _playerDetected;
    private float _loseTimer;
    private bool _isEnabled;

    private void PlayerPosition()
    {
        if (_isEnabled && DistanceToPlayer() > _enablingDistance)
        {
            _isEnabled = false;
        }
        else if (!_isEnabled && DistanceToPlayer() <= _enablingDistance)
        {
            _isEnabled = true;
        }
    }

    /// <summary>
    /// Casts a ray to the Player to check for obstacles in the path
    /// </summary>
    private void DetectPlayer()
    {
        if (!_playerWithinRange) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, _player.position - transform.position, 20f, LayerMask.GetMask("Player", "Walls"));

        // Debug
        if (hit.collider.CompareTag("Player"))
        {
            Debug.DrawRay(transform.position, (_player.position - transform.position), Color.green);
        }
        else
        {
            Debug.DrawRay(transform.position, (_player.position - transform.position), Color.red);
        }

        // If it hits the player, the player is detected
        if (hit.collider.CompareTag("Player") && !_playerDetected)
        {
            StopCoroutine("MoveAround");
            _playerDetected = true;
            FindPath();
            Debug.Log("Player Detected");
        }
    }

    /// <summary>
    /// Stops detecting the player if it exists the detection bounds during a time period
    /// </summary>
    private void LosePlayer()
    {
        if (_playerWithinRange || !_playerDetected)
        {
            _loseTimer = 0; 
            return;
        }

        _loseTimer += Time.deltaTime;
        if (_loseTimer >= _loseDetectionTime && _playerDetected)
        {
            Debug.Log("Player Lost");
            _playerDetected = false;
            StartCoroutine("MoveAround");
            _loseTimer = 0;
        }
    }
    #endregion

    #region Movement
    [Header("Movement timings")]
    [SerializeField] protected float _minWalkingTime;
    [SerializeField] protected float _maxWalkingTime;
    [SerializeField] protected float _minWaitingTime;
    [SerializeField] protected float _maxWaitingTime;
    [SerializeField] protected float _pathfindingRate;


    private float _moveTime;
    private float _moveTimer;
    private bool _isMoving;
    private bool _willTransition;
    protected Vector2 _direction;
    private float _pathTimer;
    protected List<Vector2> _pathToTake;

    private void UpdatePath()
    {
        if (!_playerDetected) return;

        _pathTimer += Time.deltaTime;
        
        if (_pathTimer > _pathfindingRate)
        {
            FindPath();
        }
    }

    private void FindPath()
    {
        Debug.Log("Find path");
        _pathTimer = 0;
        _pathToTake = Pathfinding.Instance.FindVectorPath(_rigidbody.position, _player.position);
        _pathToTake.RemoveAt(0);
        _direction = _pathToTake[0] - _rigidbody.position;
    }

    /// <summary>
    /// Moves around the map when it has not detected the player
    /// </summary>
    protected IEnumerator MoveAround()
    {
        //if (_playerDetected) return;

        /*
        if (_moveTimer < _moveTime)
        {
            if (_isMoving)
            {
                _rigidbody.velocity = _velocity * _direction;
                if (_isColiding)
                {
                    _moveTimer = _moveTime;
                    _isColiding = false;
                }
            }
            _moveTimer += Time.deltaTime;
        }
        else
        {
            _moveTimer = 0;
            _rigidbody.velocity = Vector2.zero;
            _isMoving = !_isMoving;

            _moveTime = _isMoving ? Random.Range(_minWalkingTime, _maxWalkingTime) : Random.Range(_minWaitingTime, _maxWaitingTime);

            if (_isMoving)
            {
                // Avoids walking into a wall
                while (true)
                {
                    _direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                    Debug.DrawRay(transform.position, _direction, Color.yellow);
                    if (!Physics2D.Raycast(transform.position, _direction, 1, LayerMask.GetMask("Walls")))
                    {
                        break;
                    }
                }
            }
        }
        */

        
        while (!_playerDetected)
        {
            Vector2 direction = Vector2.zero;
            bool isWall = true;
            while (isWall)
            {
                direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                Debug.DrawRay(transform.position, _direction, Color.yellow);
                isWall = Physics2D.Raycast(transform.position, _direction, 1, LayerMask.GetMask("Walls"));
            }
            _rigidbody.velocity = _velocity * direction;

            float walkingTime = Random.Range(_minWalkingTime, _maxWalkingTime);
            yield return new WaitForSeconds(walkingTime);

            _rigidbody.velocity = Vector2.zero;

            float waitingTime = Random.Range(_minWaitingTime, _maxWaitingTime);
            yield return new WaitForSeconds(waitingTime);
        }
        
    }

    private void WalkOrWait()
    {
        _isMoving = !_isMoving;
        _willTransition = false;
    }

    private void SetWalkTime()
    {
        _moveTime = Random.Range(_minWalkingTime, _maxWalkingTime);
    }

    private void SetWaitTime()
    {
        _moveTime = Random.Range(_minWalkingTime, _maxWalkingTime);
    }

    protected float DistanceToPlayer()
    {
        return Vector2.Distance(_player.transform.position, _rigidbody.position);
    }
    #endregion

    public int Cost()
    {
        return _cost;
    }
}