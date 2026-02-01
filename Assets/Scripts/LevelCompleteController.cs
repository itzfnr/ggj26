using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteController : MonoBehaviour
{
    // menu button
    public Button menuButton;
    public Button playAgainButton;
    public Button mapButton;

    // Audio stuff
    private AudioSource audioSource;
    public AudioClip levelCompleteClip;
    public AudioClip bossCompleteClip;

    public float audioVolume;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        AudioClip soundToPlay;

        if (PlayerPrefs.GetInt("IsCurrentLevelBoss") == 1)
        {
            soundToPlay = bossCompleteClip;
        } else
        {
            soundToPlay = levelCompleteClip;
        }

        audioSource.PlayOneShot(soundToPlay, audioVolume);

        string key = $"Level{PlayerPrefs.GetInt("CurrentlyPlayingLevel")}Completed";
        Debug.Log(key);
        PlayerPrefs.SetInt(key, 1);

        menuButton.onClick.AddListener(OnMenuButtonClick);
        playAgainButton.onClick.AddListener(OnPlayAgainButtonClick);
        mapButton.onClick.AddListener(OnMapButtonClick);
    }

    void OnMenuButtonClick()
    {
        SceneManager.LoadScene("MenuScene");
    }

    void OnPlayAgainButtonClick()
    {
        switch(PlayerPrefs.GetInt("CurrentlyPlayingLevel"))
        {
            case 1:
                SceneManager.LoadScene("ComponentIntegrationTest");
                break;
            case 2:
                SceneManager.LoadScene("ComponentIntegrationTest_Level2");
                break;
            case 3:
                SceneManager.LoadScene("ComponentIntegrationTest_Level3");
                break;
            default:
                SceneManager.LoadScene("MapArea");
                break;
        }
    }

    void OnMapButtonClick()
    {
        SceneManager.LoadScene("MapArea");
    }

}
