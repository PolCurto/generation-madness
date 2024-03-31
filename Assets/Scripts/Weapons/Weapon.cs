using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject {

    public new string name;

    public Sprite weaponSprite;
    public Sprite bulletSprite;

    public float fireRate;
    public float bulletSpeed;
    public float damage;
    public float bulletsPerShot;

}
