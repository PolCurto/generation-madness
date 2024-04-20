using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLogicController : MonoBehaviour
{
    [HideInInspector] public static FloorLogicController Instance;

    [SerializeField] private GameObject _portal;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnBossDeath(Vector3 position)
    {
        Instantiate(_portal, position, Quaternion.identity);
    }

    #region Temple Methods
    public virtual void OnPlayerEnterRoom(TempleRoom room) { }
    #endregion


}
