using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private int _maxLife;

    private int _currentLife;

    private void Start()
    {
        _currentLife = _maxLife;
        UIController.Instance.UpdateLife(_maxLife, _currentLife);
    }

    public void GetHit(int damage)
    {
        _currentLife -= damage;
        Debug.Log(_currentLife);

        UIController.Instance.UpdateLife(_maxLife, _currentLife);
    }
}
