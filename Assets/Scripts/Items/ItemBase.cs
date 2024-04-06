using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Passive Item")]
public class ItemBase : ScriptableObject
{
    public new string name;

    public Sprite itemSprite;
    public int life;
    public bool healOnObtain;
    public float attack;
    public float speed;
    public float attackSpeed;
    public float reloadSpeed;
    public float bulletSpeed;
}
