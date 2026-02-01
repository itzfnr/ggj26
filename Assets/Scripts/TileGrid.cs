using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileGrid : MonoBehaviour
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

    public float refreshDelay = 1.0f;
    private bool refreshActive;

    public bool haltAction;

    public List<GameObject> tiles; // List of tiles currently active on the grid

    public List<Sprite> availableTileSprites;

    // AudioClips for UI.
    public AudioClip uiSelectSound;
    public AudioClip uiDeselectSound;
    public AudioClip uiMoveSound;

    // AudioSource
    private AudioSource audioSource;

    // public for volume control of UI
    public float uiVolume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // Get audiosource.
        audioSource = gameObject.GetComponent<AudioSource>();

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
        Sprite[] previousLeft = new Sprite[ySize]; // Array of previous sprites on the left side
        Sprite previousBelow = null; // Previous sprite on the bottom side

        for (int x = -xSize / 2; x < xSize / 2; x++)
        {
            for (int y = -ySize / 2; y < ySize / 2; y++)
            {
                Vector3 newTilePosition = new Vector3((1 * x) - xOffset, (1 * y) - yOffset, tile.transform.position.z); // Calculate position of new tile with offset
                GameObject newTile = Instantiate(tile, newTilePosition, tile.transform.rotation); // Create a tile
                tiles.Add(newTile); // Add newly created tile to the list of tiles
                newTile.transform.parent = transform; // Set the parent of the tile to the Board

                List<Sprite> possibleSprites = new List<Sprite>(); // Create a list of possible sprites
                possibleSprites.AddRange(availableTileSprites); // Add the sprites to the range
                possibleSprites.Remove(previousLeft[(ySize / 2) + y]); // Remove possible sprites by reading the sprites of the tiles to the left
                possibleSprites.Remove(previousBelow); // Same as above but just for the one below
                Sprite newSprite = possibleSprites[Random.Range(0, possibleSprites.Count)]; // Randomise the sprite between the minimum and maximum of the remaining possible sprites
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite; // Set the sprite of the tile to the randomised sprite

                previousLeft[(ySize / 2) + y] = newSprite; // Set the previous left to the new sprite for the next loop
                previousBelow = newSprite; // Same as above for the one below
            }
        }
    }

    private void Update()
    {
        if (!haltAction && moveTimer <= 0.0f && (pointerMove.ReadValue<Vector2>().x != 0 || pointerMove.ReadValue<Vector2>().y != 0))
        {
            pointerPosition += pointerMove.ReadValue<Vector2>();
            if (previousTile == null)
            {
                pointerPosition.x = Mathf.Clamp(pointerPosition.x, 
                    (-xSize / 2) - xOffset, 
                    (xSize / 2) - 1 - xOffset);
                pointerPosition.y = Mathf.Clamp(pointerPosition.y, 
                    (-ySize / 2) - yOffset, 
                    (ySize / 2) - 1 - yOffset);
            }
            else
            {
                pointerPosition.x = Mathf.Clamp(pointerPosition.x, 
                    previousTile.transform.position.x <= (-xSize / 2) - xOffset ? previousTile.transform.position.x : previousTile.transform.position.x - 1,
                    previousTile.transform.position.x >= (xSize / 2) - 1 - xOffset ? previousTile.transform.position.x : previousTile.transform.position.x + 1);
                pointerPosition.y = Mathf.Clamp(pointerPosition.y,
                    previousTile.transform.position.y <= (-ySize / 2) - yOffset ? previousTile.transform.position.y : previousTile.transform.position.y - 1,
                    previousTile.transform.position.y >= (ySize / 2) - 1 - yOffset ? previousTile.transform.position.y : previousTile.transform.position.y + 1);
            }
            pointer.transform.position = pointerPosition;
            moveTimer = moveDelay;

            // play move oneshot
            audioSource.PlayOneShot(uiMoveSound, uiVolume);
        }

        if (moveTimer > 0.0f)
        {
            moveTimer -= Time.deltaTime;
        }

        if (!haltAction && selectTile.triggered)
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
                            // deselect
                            audioSource.PlayOneShot(uiDeselectSound, uiVolume);
                            tileObject.Deselect();
                            previousTile = null;
                            pointer.GetComponent<SpriteRenderer>().sprite = pointerDefault;
                        }
                        else
                        {
                            if (tile.GetComponent<SpriteRenderer>().sprite != null)
                            {
                                if (previousTile == null)
                                {
                                    tileObject.Selected();
                                    // on select play selected sound.
                                    audioSource.PlayOneShot(uiSelectSound, uiVolume);
                                    previousTile = tile;
                                    pointer.GetComponent<SpriteRenderer>().sprite = pointerSelected;
                                }
                                else
                                {
                                    // deselect.
                                    audioSource.PlayOneShot(uiDeselectSound, uiVolume);
                                    TileSwap(previousTile, tile); // Perform the swap
                                    previousTile.GetComponent<Tile>().Deselect();
                                    previousTile = null;
                                    pointer.GetComponent<SpriteRenderer>().sprite = pointerDefault;
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
        if (tileA.GetComponent<SpriteRenderer>().sprite == tileB.GetComponent<SpriteRenderer>().sprite)
            return;

        // Swap the sprites
        Sprite tempSprite = tileA.GetComponent<SpriteRenderer>().sprite;
        tileA.GetComponent<SpriteRenderer>().sprite = tileB.GetComponent<SpriteRenderer>().sprite;
        tileB.GetComponent<SpriteRenderer>().sprite = tempSprite;

        tileA.GetComponent<Tile>().ClearAllMatches();
        tileB.GetComponent<Tile>().ClearAllMatches();
    }

    public IEnumerator Matched()
    {
        refreshActive = true;
        haltAction = true;

        yield return new WaitForSeconds(refreshDelay);

        foreach (GameObject tile in tiles)
        {
            Destroy(tile);
        }
        tiles = new List<GameObject>();
        CreateGrid();
        refreshActive = false;
        haltAction = false;
    }
}
