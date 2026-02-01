using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isSelected;
    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private SpriteRenderer spriteRenderer;
    private bool matchFound = false;

    public AttackManager attackManager;
    public TileGrid tileGrid;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get attack manager.
        GameObject levelController = GameObject.Find("LevelController");
        attackManager = levelController.GetComponent<AttackManager>();

        GameObject board = GameObject.Find("Board");
        tileGrid = board.GetComponent<TileGrid>();
    }

    public void Selected()
    {
        isSelected = true;
    }

    public void Deselect()
    {
        isSelected = false;
    }

    private GameObject GetAdjacent(Vector2 castDir)
    {
        // Cast from slightly outside this tile to avoid hitting self
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3)(castDir * 0.5f), castDir, 1f);
        if (hit.collider != null && hit.collider.gameObject != gameObject)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            GameObject adjacent = GetAdjacent(adjacentDirections[i]);
            if (adjacent != null) // Only add non-null tiles
            {
                adjacentTiles.Add(adjacent);
            }
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        Vector3 currentPos = transform.position;

        // Start raycast from slightly offset position
        RaycastHit2D hit = Physics2D.Raycast(currentPos + (Vector3)(castDir * 0.5f), castDir, 1f);

        while (hit.collider != null &&
               hit.collider.gameObject != gameObject &&
               hit.collider.GetComponent<SpriteRenderer>() != null &&
               hit.collider.GetComponent<SpriteRenderer>().sprite != null &&
               hit.collider.GetComponent<SpriteRenderer>().sprite == spriteRenderer.sprite) // CHANGED: Compare sprites instead of colors
        {
            matchingTiles.Add(hit.collider.gameObject);
            currentPos = hit.collider.transform.position;
            hit = Physics2D.Raycast(currentPos + (Vector3)(castDir * 0.5f), castDir, 1f);
        }

        return matchingTiles;
    }

    private void ClearMatch(Vector2[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }

        if (matchingTiles.Count >= 2)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
                tileGrid.CreateParticles(gameObject.GetComponent<SpriteRenderer>().sprite.name, gameObject);
                tileGrid.CreateParticles(gameObject.GetComponent<SpriteRenderer>().sprite.name, matchingTiles[i]);
            }
            matchFound = true;
        }
    }

    public void ClearAllMatches()
    {
        if (spriteRenderer.sprite == null)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });

        if (matchFound)
        {
            attackManager.OnAttack(gameObject.GetComponent<SpriteRenderer>().sprite.name);
            StartCoroutine(tileGrid.Matched());
            spriteRenderer.sprite = null;
            matchFound = false;
        }
    }
}