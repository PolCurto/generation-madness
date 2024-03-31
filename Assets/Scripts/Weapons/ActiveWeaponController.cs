using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeaponController : MonoBehaviour
{
    [SerializeField] private Weapon _weapon;
    [SerializeField] private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(_weapon.name);
    }

    // Update is called once per frame
    void Update()
    {
        RotateOut();   
    }

    private void RotateOut()
    {
        if (transform.position.x - _player.transform.position.x < 1)
        {
            transform.localEulerAngles = new Vector3(180, 0, 0);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
        }
    }
}
