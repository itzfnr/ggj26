using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMap : MonoBehaviour
{
    [SerializeField] private int levelToMarkCompleted = 1;
    [SerializeField] private string mapSceneName = "Map_area";

    public void GoBack()
    {
        Debug.Log($"GoBack() called. Scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} levelToMarkCompleted={levelToMarkCompleted}");

        string key = $"Level{levelToMarkCompleted}Completed";

        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        Debug.Log($"SAVED NOW: {key} = {PlayerPrefs.GetInt(key, 0)}");

        SceneManager.LoadScene(mapSceneName);
    }
}
