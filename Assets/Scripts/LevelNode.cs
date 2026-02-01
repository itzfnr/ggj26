using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNode : MonoBehaviour
{
    [Header("Which scene this node loads")]
    [SerializeField] private string sceneName;

    // Optional: keep this if you're using it elsewhere
    [SerializeField] public int levelIndex = 1;

    public void Click()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"{name}: sceneName is empty! Set it in the Inspector.");
            return;
        }

        Debug.Log($"{name} loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
