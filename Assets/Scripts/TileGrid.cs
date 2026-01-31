using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grid : MonoBehaviour
{
    private int xSize, ySize; // X and Y size of the grid

    public float xOffset, yOffset; // X and Y offset of the grid

    public GameObject tile; // Tile prefab for creating the tile grid
    public GameObject pointer;

    private GameObject previousTile;

    public InputActionAsset inputs;
    private InputAction pointerMove;
    private InputAction selectTile;

    private Vector2 pointerPosition;

    public Sprite pointerDefault;
    public Sprite pointerSelected;

    private float moveTimer = 0.0f;
    public float moveDelay = 0.125f;

    public float refreshDelay = 3.0f;

    public List<Color> colors; // Possible colours that a tile can be
    public List<GameObject> tiles; // List of tiles currently active on the grid

    // Start is called before the first frame update
    void Start()
    {
        xSize = (int)transform.localScale.x; // Set the X size to the X scale of the grid
        ySize = (int)transform.localScale.y; // Set the Y size to the Y scale of the grid

        pointerMove = inputs.FindAction("PointerMove");
        selectTile = inputs.FindAction("SelectTile");

        pointerPosition = new Vector3((-xSize / 2) - xOffset, (-ySize / 2) - yOffset, pointer.transform.position.z);
        pointer.transform.position = pointerPosition;

        CreateGrid();
    }

    private void CreateGrid()
    {
        Color[] previousLeft = new Color[ySize]; // Array of previous colours on the left side
        Color previousBelow = default; // Previous colour on the bottom side

        for (int x = -xSize / 2; x < xSize / 2; x++)
        {
            for (int y = -ySize / 2; y < ySize / 2; y++)
            {
                Vector3 newTilePosition = new Vector3((1 * x) - xOffset, (1 * y) - yOffset, tile.transform.position.z); // Calculate position of new tile with offset
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

    private void Update()
    {
        if (moveTimer <= 0.0f && (pointerMove.ReadValue<Vector2>().x != 0 || pointerMove.ReadValue<Vector2>().y != 0))
        {
            pointerPosition += pointerMove.ReadValue<Vector2>();
            if (previousTile == null)
            {
                pointerPosition.x = Mathf.Clamp(pointerPosition.x, (-xSize / 2) - xOffset, (xSize / 2) - 1 - xOffset);
                pointerPosition.y = Mathf.Clamp(pointerPosition.y, (-ySize / 2) - yOffset, (ySize / 2) - 1 - yOffset);
            }
            else
            {
                pointerPosition.x = Mathf.Clamp(pointerPosition.x, previousTile.transform.position.x - 1, previousTile.transform.position.x + 1);
                pointerPosition.y = Mathf.Clamp(pointerPosition.y, previousTile.transform.position.y - 1, previousTile.transform.position.y + 1);
            }
            pointer.transform.position = pointerPosition;
            moveTimer = moveDelay;
        }

        if (moveTimer > 0.0f)
        {
            moveTimer -= Time.deltaTime;
        }

        if (selectTile.triggered)
        {
            foreach (GameObject tile in tiles)
            {
                // Check if the pointer is on the tile
                if (tile.transform.position == pointer.transform.position)
                {
                    Tile tileObject = tile.GetComponent<Tile>();

                    if (tileObject != null)
                    {
                        // If the tile is already selected, deselect it
                        if (tileObject.isSelected)
                        {
                            tileObject.Deselect();
                            pointer.GetComponent<SpriteRenderer>().sprite = pointerDefault;
                        }
                        else
                        {
                            if (tile.GetComponent<SpriteRenderer>().sprite != null)
                            {
                                if (previousTile == null)
                                {
                                    tileObject.Selected();
                                    previousTile = tile;
                                    pointer.GetComponent<SpriteRenderer>().sprite = pointerSelected;
                                }
                                else
                                {
                                    TileSwap(previousTile, tile); // Perform the swap
                                    previousTile.GetComponent<Tile>().Deselect();
                                    previousTile = null;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void TileSwap(GameObject tileA, GameObject tileB)
    {
        if (tileA.GetComponent<SpriteRenderer>().color == tileB.GetComponent<SpriteRenderer>().color)
            return;

        // Swap the colors
        Color tempColor = tileA.GetComponent<SpriteRenderer>().color;
        tileA.GetComponent<SpriteRenderer>().color = tileB.GetComponent<SpriteRenderer>().color;
        tileB.GetComponent<SpriteRenderer>().color = tempColor;

        tileA.GetComponent<Tile>().ClearAllMatches();
        tileB.GetComponent<Tile>().ClearAllMatches();

        pointer.GetComponent<SpriteRenderer>().sprite = pointerDefault;

        StartCoroutine(Refresh());
    }

    IEnumerator Refresh()
    {
        yield return new WaitForSeconds(refreshDelay);
        tiles = new List<GameObject>();
        CreateGrid();
    }
}
