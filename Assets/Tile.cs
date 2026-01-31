using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isSelected;

    private Vector3 beforePosition;
    private Color beforeColor;

    //public GameObject previousTile;
    private GameObject pointer;

    private SpriteRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void Selected()
    {
        isSelected = true;
    }

    public void Deselect()
    {
        isSelected = false;
    }
}
