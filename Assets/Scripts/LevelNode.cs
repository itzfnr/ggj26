using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelNode : MonoBehaviour
{
    public int levelIndex = 1;

    public void Click()
    {
        Debug.Log("Clicked level: " + levelIndex);
    }
}
