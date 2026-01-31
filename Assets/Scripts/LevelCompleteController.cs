using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteController : MonoBehaviour
{
    // menu button
    public Button menuButton;

    // Start is called before the first frame update
    void Start()
    {
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
