using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Global Variables
    [SerializeField] protected int _cost;
    [Header("Global Stats")]
    [SerializeField] protected int _maxLife;
    [SerializeField] protected float _velocity;
    [SerializeField] protected float _loseDetectionTime;

    protected Transform _player;
    protected bool _playerDetected;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _player = GameObject.Find("Player").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) _playerDetected = true;
    }
    #endregion



    public int Cost()
    {
        return _cost;
    }




    
}
