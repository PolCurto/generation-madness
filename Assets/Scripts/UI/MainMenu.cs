using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   

    public void GenerateCaveLevel()
    {
        SceneManager.LoadScene("Cave Floor");
    }

    public void GenerateDungeonLevel()
    {
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        SceneManager.LoadScene("Dungeon Floor");
    }
}
