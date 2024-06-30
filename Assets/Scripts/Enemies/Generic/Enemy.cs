using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    #region Global Variables
    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected AudioSource _audioSource;

    [Header("Audios")]
    [SerializeField] protected AudioClip _damageAudio;
    [SerializeField] protected AudioClip _shootAudio;

    [Header("Global Stats")]
    [SerializeField] protected int _cost;
    [SerializeField] protected float _velocity;
    [SerializeField] protected float _maxVelocity;
    [SerializeField] protected float _detectionDistance;
    [SerializeField] protected float _enablingDistance;
    [SerializeField] protected float _attackDistance;
    [SerializeField] protected float _updatePathRate;
    [SerializeField] protected float _nextWaypointDistance;

    protected Rigidbody2D _player;
    protected Animator _animator;
    protected float _timer;
    private bool _playerInSight;
    private bool _isColiding;

    // Pathfinding
    protected Path _path;
    protected int _currentWaypoint;
    protected bool _reachedEndOfPath;
    protected Seeker _seeker;

    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        _pathToTake = new List<Vector2>();
        _animator = GetComponent<Animator>();
        _seeker = GetComponent<Seeker>();
        _spriteRenderer.material = new Material(_spriteRenderer.material);
    }

    protected void Start()
    {
        InvokeRepeating(nameof(CalculatePath), 0, _updatePathRate);
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
        if (_playerWithinRange)  // If the player is within the enemy detection range, checks if is on sight
        {
            CheckPlayerInSight();
            
            if (_playerInSight && !_playerDetected)  // If it hits the player, the player is detected
            {
                _playerDetected = true;
            }
        }
        
        if (_playerDetected && !_playerWithinRange)  // If the player is detected and outside the detection range, stops detecting it after a certain time
        {
            _loseTimer += Time.deltaTime;
            if (_loseTimer >= _loseDetectionTime && _playerDetected)
            {
                Debug.Log("Player Lost");
                _playerDetected = false;
                _loseTimer = 0;
            }
        }
        else
        {
            _loseTimer = 0;
            return;
        }
    }

    /// <summary>
    /// Checks if the player is within its line of vision
    /// </summary>
    private void CheckPlayerInSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _player.position - _rigidbody.position, 20f, LayerMask.GetMask("Player", "Walls"));

        // Debug
        if (hit && hit.collider != null)
        {
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
    }
    #endregion

    #region Movement
    [Header("Movement timings")]
    [SerializeField] protected float _minWalkingTime;
    [SerializeField] protected float _maxWalkingTime;
    [SerializeField] protected float _minWaitingTime;
    [SerializeField] protected float _maxWaitingTime;

    private float _moveTime;
    private float _moveTimer;
    private bool _isWalking;
    private bool _willTransition;
    protected Vector2 _direction;
    private float _pathTimer;
    protected List<Vector2> _pathToTake;
    protected bool _canAttack;

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

        Vector2 moveForce = _velocity * Time.deltaTime * _direction;
        _rigidbody.velocity = moveForce;

        _moveTimer += Time.deltaTime;

        if (_moveTimer > _moveTime)
        {
            _moveTimer = 0;
            ChangeDirection();
        }
    }

    /// <summary>
    /// Changes the direction where the enemy will head the next time it moves
    /// </summary>
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

    /// <summary>
    /// Gets a valid dirction, avoiding walking into walls
    /// </summary>
    /// <returns></returns>
    private Vector2 GetValidDirection()
    {
        Vector2 direction;

        // Avoids walking into a wall
        int iterations = 0;
        while (true)
        {
            direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            Debug.DrawRay(transform.position, direction, Color.yellow);
            if (!Physics2D.Raycast(transform.position, direction, 2, LayerMask.GetMask("Walls")) || iterations > 100)
            {
                break;
            }

            // Avoid a possible infinite loop
            iterations++;
        }

        return direction;
    }

    protected void CalculatePath()
    {
        if (!_isEnabled) return;

        if (_seeker.IsDone())
        {
            _seeker.StartPath(_rigidbody.position, _player.position, OnPathComplete);
        }
    }

    protected void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            _path = path;
            _currentWaypoint = 0;
        }
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

        if (_canAttack && (DistanceToPlayer() > _attackDistance || !_playerInSight))
        {
            _canAttack = false;
            return;
        }

        if (!_canAttack && DistanceToPlayer() <= _attackDistance && _playerInSight)
        {
            _canAttack = true;
        }
    }
    #endregion

    #region Damage
    public void GetHit()
    {
        if (!gameObject.activeSelf) return;

        StartCoroutine(FlashWhite());
        _audioSource.PlayOneShot(_damageAudio);
    }

    private IEnumerator FlashWhite()
    {
        _spriteRenderer.material.SetFloat("_FlashAmount", 1);

        yield return new WaitForSeconds(0.05f);

        _spriteRenderer.material.SetFloat("_FlashAmount", 0);
    }
    public virtual void Die()
    {
        gameObject.SetActive(false);
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