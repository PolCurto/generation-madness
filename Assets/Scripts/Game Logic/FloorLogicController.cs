using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLogicController : MonoBehaviour
{
    [HideInInspector] public static FloorLogicController Instance;

    [SerializeField] private GameObject _nextLevelPortal;
    [SerializeField] private GameObject _gameEndPortal;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnBossDeath(Vector3 position)
    {
        // End the game if we are in floor 6, or if we are in floor 3 of a game restricted to a level type
        if (LevelsLoader.Instance.FloorDepth >= 5 || (LevelsLoader.Instance.RunType != LevelsLoader.RunLevelsType.Default && LevelsLoader.Instance.FloorDepth >= 2))
        {
            Instantiate(_gameEndPortal, position, Quaternion.identity);
        }
        else
        {
            Instantiate(_nextLevelPortal, position, Quaternion.identity);
        }
    }

    #region Temple Methods
    public virtual void OnPlayerEnterRoom(TempleRoom room) { }
    #endregion
}
