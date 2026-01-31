using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private int xSize, ySize; // X and Y size of the grid

    public float xOffset, yOffset; // X and Y offset of the grid

    public GameObject tile; // Tile prefab for creating the tile grid

    public List<Color> colors; // Possible colours that a tile can be
    public List<GameObject> tiles; // List of tiles currently active on the grid

    // Start is called before the first frame update
    void Start()
    {
        xSize = (int)transform.localScale.x; // Set the X size to the X scale of the grid
        ySize = (int)transform.localScale.y; // Set the Y size to the Y scale of the grid

        Color[] previousLeft = new Color[ySize]; // Array of previous colours on the left side
        Color previousBelow = default; // Previous colour on the bottom side

        for (int x = -xSize / 2; x < xSize / 2; x++)
        {
            for (int y = -ySize / 2; y < ySize / 2; y++)
            {
                Vector3 newTilePosition = new Vector3((1 * x)-xOffset, (1 * y)-yOffset, tile.transform.position.z); // Calculate position of new tile with offset
                GameObject newTile = Instantiate(tile, newTilePosition, tile.transform.rotation); // Create a tile
                tiles.Add(newTile); // Add newly created tile to the list of tiles
                newTile.transform.parent = transform; // Set the parent of the tile to the Board

                List<Color> possibleColors = new List<Color>(); // Create a list of possible colours
                possibleColors.AddRange(colors); // Add the colours to the range
                possibleColors.Remove(previousLeft[(ySize / 2) + y]); // Remove possible colours by reading the colours of the tiles to the left
                possibleColors.Remove(previousBelow); // Same as above but just for the one below
                Color newColor = possibleColors[Random.Range(0, possibleColors.Count)]; // Randomise the colour between the minimum and maximum of the remaining possible colours
                newTile.GetComponent<SpriteRenderer>().color = newColor; // Set the colour of the tile to the randomised colour

                previousLeft[(ySize / 2) + y] = newColor; // Set the previous left to the new possible colours for the next loop
                previousBelow = newColor; // Same as above for the one below
            }
        }
    }
}
