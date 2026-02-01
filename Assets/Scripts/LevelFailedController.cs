using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelFailedController : MonoBehaviour
{
    // menu button
    public Button menuButton;

    // Audio stuff
    private AudioSource audioSource;
    public AudioClip failClip;

    public float audioVolume;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        audioSource.PlayOneShot(failClip, audioVolume);

        menuButton.onClick.AddListener(OnMenuButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void OnMenuButtonClick()
    {
        SceneManager.LoadScene("MenuScene");
    }
    
}
