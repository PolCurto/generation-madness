using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponBase : ScriptableObject
{
    [Header("Weapon")]
    public int id;
    public new string name;
    public float cameraOffsetMultiplier;

    public Sprite weaponSprite;
    public float fireRate;
    public int maxBullets;
    public int clipSize;
    public float dispersion;
    public Color color;
    public AnimatorOverrideController overrideController;
    public AudioClip shotAudio;
    //public AudioClip reloadAudio;
    

    [Header("Bullets")]
    public Sprite bulletSprite;
    public float bulletSpeed;
    public int bulletDamage;
    public float bulletDuration;
    public int bulletsPerShot;
}
