using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressionManager : MonoBehaviour
{
    [SerializeField] public int currentLevel;
    [SerializeField] public bool isCurrentLevelBoss;

    // Start is called before the first frame update
    void Start()
    {
        // on start set our playerprefs current level based on our level number
        PlayerPrefs.SetInt("CurrentlyPlayingLevel", currentLevel);
        PlayerPrefs.SetInt("IsCurrentLevelBoss", isCurrentLevelBoss ? 1 : 0);
    }
}
