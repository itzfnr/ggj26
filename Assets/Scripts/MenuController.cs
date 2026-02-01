using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    // play button
    public Button playButton;

    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void OnPlayButtonClick()
    {
        SceneManager.LoadScene("MapArea");
    }
    
}
