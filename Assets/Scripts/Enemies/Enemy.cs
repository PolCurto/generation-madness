using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int _cost;
    [Header("Global Stats")]
    [SerializeField] protected int _maxLife;

    public int Cost()
    {
        return _cost;
    }
    
}
