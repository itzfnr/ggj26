using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapProgress : MonoBehaviour
{
    [Header("Node 2")]
    public Button node2Button;
    public GameObject line_1_2;

    [Header("Node 3")]
    public Button node3Button;
    public GameObject line_2_3;

    void Start()
    {
        int level1Completed = PlayerPrefs.GetInt("Level1Completed", 0);
        int level2Completed = PlayerPrefs.GetInt("Level2Completed", 0);

        Debug.Log($"Level1Completed={level1Completed} Level2Completed={level2Completed}");

        // ---- NODE 2 ----
        node2Button.interactable = false;
        node2Button.GetComponent<Image>().color = Color.gray;
        if (line_1_2) line_1_2.SetActive(false);

        if (level1Completed == 1)
        {
            node2Button.interactable = true;
            node2Button.GetComponent<Image>().color = Color.white;
            if (line_1_2) line_1_2.SetActive(true);
        }

        // ---- NODE 3 ----
        node3Button.interactable = false;
        node3Button.GetComponent<Image>().color = Color.gray;
        if (line_2_3) line_2_3.SetActive(false);

        if (level2Completed == 1)
        {
            node3Button.interactable = true;
            node3Button.GetComponent<Image>().color = Color.white;
            if (line_2_3) line_2_3.SetActive(true);
        }
    }
}


