using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelNode : MonoBehaviour
{
    public int levelIndex = 1;

    public void Click()
    {
        //string sceneName = "Level" + levelIndex;
        //SceneManager.LoadScene(sceneName);

        // TEMP: all nodes load the same prototype level
        SceneManager.LoadScene("ComponentIntegrationTest");
    }
}
