using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private int xSize, ySize;

    public GameObject tile;

    public List<Color> colors;
    public List<GameObject> tiles;

    // Start is called before the first frame update
    void Start()
    {
        xSize = (int)transform.localScale.x;
        ySize = (int)transform.localScale.y;

        for (int x = -xSize / 2; x < xSize / 2; x++)
        {
            for (int y = -ySize / 2; y < ySize / 2; y++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(1 * x, 1 * y, tile.transform.position.z), tile.transform.rotation); // Create a tile
                tiles.Add(newTile); // Add newly created tile to the list of tiles
                newTile.transform.parent = transform; // Set the parent of the tile to the Board
                Color newColor = colors[Random.Range(0, colors.Count)]; // Randomise the colour between the minimum and maximum possible values of colours
                newTile.GetComponent<SpriteRenderer>().color = newColor; // Set the colour of the tile to the randomised colour
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
