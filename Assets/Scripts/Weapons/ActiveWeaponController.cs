using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Weapon _weapon;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        RotateOut();   
    }

    private void RotateOut()
    {
        if (transform.position.x - _player.transform.position.x < 0)
        {
            transform.localEulerAngles = new Vector3(180, 0, 0);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
        }
    }

    public void SwapWeapon(Weapon weapon)
    {
        _weapon = weapon;
        _spriteRenderer.sprite = _weapon.itemSprite;
    }

    #region Shooting
    public void Shoot()
    {

    }

    #endregion

    public Weapon Weapon => _weapon;
}
