using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Global Variables
    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected SpriteRenderer _spriteRenderer;

    [Header("Global Stats")]
    [SerializeField] protected int _cost;
    [SerializeField] protected float _maxVelocity;
    [SerializeField] protected float _acceleration;
    [SerializeField] protected float _detectionDistance;
    [SerializeField] protected float _enablingDistance;
    [SerializeField] protected float _attackDistance;

    protected Rigidbody2D _player;
    protected Animator _animator;
    protected float _timer;
    private bool _playerInSight;
    private bool _isColiding;
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        _pathToTake = new List<Vector2>();
        _animator = GetComponent<Animator>();
    }

    protected void Start()
    {
        //StartCoroutine("MoveAround");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && !_playerDetected)
        {
            //StopCoroutine("MoveAround");
            //StartCoroutine("MoveAround");
        } 
    }

    protected virtual void Update()
    {
        CheckPlayerPosition();
        CheckPlayerWithinRange();
        FlipSprite();

        _animator.SetFloat("Velocity", Mathf.Abs(_rigidbody.velocity.magnitude));

        if (!_isEnabled) return;

        for (int i = 0; i < _pathToTake.Count - 1; i++)
        {
            Debug.DrawLine(_pathToTake[i], _pathToTake[i + 1]);
        }

        _timer += Time.deltaTime;
    }

    protected virtual void FixedUpdate()
    {
        if (!_isEnabled) return;

        PlayerDetection();
        SetAttackStatus();
        //UpdatePath();
        MoveAround();
    }
    #endregion

    #region Player Detection
    [SerializeField] protected float _loseDetectionTime;

    protected bool _playerWithinRange;
    protected bool _playerDetected;
    private float _loseTimer;
    private bool _isEnabled;

    private void CheckPlayerPosition()
    {
        if (_isEnabled && DistanceToPlayer() > _enablingDistance)
        {
            _isEnabled = false;
            _rigidbody.simulated = false;
        }
        else if (!_isEnabled && DistanceToPlayer() <= _enablingDistance)
        {
            _isEnabled = true;
            _rigidbody.simulated = true;
        }
    }

    private void CheckPlayerWithinRange()
    {
        if (DistanceToPlayer() < _detectionDistance)
        {
            _playerWithinRange = true;
        } 
        else
        {
            _playerWithinRange = false;
        }
    }

    /// <summary>
    /// Casts a ray to the Player to check for obstacles in the path
    /// </summary>
    private void PlayerDetection()
    {
        // If the player is within the enemy detection range, checks if is on sight
        if (_playerWithinRange)
        {
            CheckPlayerInSight();
            // If it hits the player, the player is detected
            if (_playerInSight && !_playerDetected)
            {
                //StopCoroutine("MoveAround");
                _playerDetected = true;
                //FindPath();
                //Debug.Log("Player Detected");
            }
        }

        // If the player is detected and outside the detection range, stops detecting it after a certain time
        if (_playerDetected && !_playerWithinRange)
        {
            _loseTimer += Time.deltaTime;
            if (_loseTimer >= _loseDetectionTime && _playerDetected)
            {
                Debug.Log("Player Lost");
                _playerDetected = false;
                //StartCoroutine("MoveAround");
                _loseTimer = 0;
            }
        }
        else
        {
            _loseTimer = 0;
            return;
        }
    }

    private void CheckPlayerInSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _player.position - _rigidbody.position, 20f, LayerMask.GetMask("Player", "Walls"));

        // Debug
        if (hit.collider.CompareTag("Player"))
        {
            Debug.DrawRay(transform.position, (_player.position - _rigidbody.position), Color.green);
        }
        else
        {
            Debug.DrawRay(transform.position, (_player.position - _rigidbody.position), Color.red);
        }

        _playerInSight = hit.collider.CompareTag("Player");
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
    private bool _isWalking;
    private bool _willTransition;
    protected Vector2 _direction;
    private float _pathTimer;
    protected List<Vector2> _pathToTake;
    protected bool _isAttacking;

    /*
    private void UpdatePath()
    {
        if (!_playerDetected) return;

        _pathTimer += Time.deltaTime;
        
        if (_pathTimer > _pathfindingRate || _pathToTake.Count <= 1)
        {
            FindPath();
        }
    }

    private void FindPath()
    {
        _pathTimer = 0;
        _pathToTake = Pathfinding.Instance.FindVectorPath(_rigidbody.position, _player.position);

        if (_pathToTake == null)
        {
            Debug.Log("Path null");
            return;
        }
        _pathToTake.RemoveAt(0);
    }
    */

    protected void FlipSprite()
    {
        if (_rigidbody.velocity.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (_rigidbody.velocity.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
    }

    /// <summary>
    /// Moves around the map when it has not detected the player
    /// </summary>
    protected void MoveAround()
    {
        if (_playerDetected) return;

        Vector2 moveForce = Vector2.MoveTowards(_rigidbody.velocity, _direction * _maxVelocity, _acceleration * Time.fixedDeltaTime);
        _rigidbody.velocity = moveForce;

        _moveTimer += Time.deltaTime;

        if (_moveTimer > _moveTime)
        {
            _moveTimer = 0;
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        _isWalking = !_isWalking;

        if (_isWalking)
        {
            _moveTime = Random.Range(_minWalkingTime, _maxWalkingTime);
            _direction = GetValidDirection().normalized;
        }
        else
        {
            _moveTime = Random.Range(_minWaitingTime, _maxWaitingTime);
            _direction = Vector2.zero;
        }
    }

    private Vector2 GetValidDirection()
    {
        Vector2 direction;

        // Avoids walking into a wall
        int iterations = 0;
        while (true)
        {
            direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            Debug.DrawRay(transform.position, direction, Color.yellow);
            if (!Physics2D.Raycast(transform.position, direction, 1, LayerMask.GetMask("Walls")) || iterations > 100)
            {
                break;
            }

            // Avoid a possible infinite loop
            iterations++;
        }

        return direction;
    }

    /*
    protected IEnumerator MoveAround()
    {
        //if (_playerDetected) return;

        
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
    */
    #endregion

    #region Attack
    protected void SetAttackStatus()
    {
        if (!_playerDetected) return;

        if (_isAttacking && (DistanceToPlayer() > _attackDistance || !_playerInSight))
        {
            _isAttacking = false;
            return;
        }

        if (!_isAttacking && DistanceToPlayer() <= _attackDistance)
        {
            _isAttacking = true;
            //Debug.Log("Attack");
        }
    }
    #endregion

    protected float DistanceToPlayer()
    {
        return Vector2.Distance(_player.transform.position, _rigidbody.position);
    }
    

    public int Cost()
    {
        return _cost;
    }
}