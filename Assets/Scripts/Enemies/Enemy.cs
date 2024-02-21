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

    protected Transform _player;

    #endregion

    #region Unity Methods
    private void Awake()
    {
        _player = GameObject.Find("Player").transform;
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

    protected void Start()
    {
        StartCoroutine(MoveAround());
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        DetectPlayer();
        LoosePlayer();
    }
    #endregion

    #region Player Detection
    [SerializeField] protected float _loseDetectionTime;

    protected bool _playerWithinRange;
    protected bool _playerDetected;
    private float _loseTimer;

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
            StopCoroutine(MoveAround());
            _playerDetected = true;
            Debug.Log("Player Detected");
        }
    }

    /// <summary>
    /// Stops detecting the player if it exists the detection bounds during a time period
    /// </summary>
    private void LoosePlayer()
    {
        if (_playerWithinRange || !_playerDetected)
        {
            _loseTimer = 0; 
            return;
        }

        _loseTimer += Time.deltaTime;
        if (_loseTimer >= _loseDetectionTime)
        {
            Debug.Log("Player Lost");
            _playerDetected = false;
            StartCoroutine(MoveAround());
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

    /// <summary>
    /// Moves around the map when it has not detected the player
    /// </summary>
    protected IEnumerator MoveAround()
    {
        while (!_playerDetected)
        {
            Debug.Log("Move Around");

            Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            _rigidbody.velocity = _velocity * direction * Time.deltaTime * 100;

            float walkingTime = Random.Range(_minWalkingTime, _maxWalkingTime);
            yield return new WaitForSeconds(walkingTime);

            _rigidbody.velocity = Vector2.zero;
            Debug.Log("Wait");

            float waitingTime = Random.Range(_minWaitingTime, _maxWaitingTime);
            yield return new WaitForSeconds(waitingTime);
        }
    }
    #endregion

    public int Cost()
    {
        return _cost;
    }
}
