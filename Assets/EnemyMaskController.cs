using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // Must have this!

public class EnemyMaskController : MonoBehaviour
{
    // the text object.
    public TMP_Text narrationText;

    public string[] taunts;
    public Texture2D[] maskTextures;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnDebugSwitchMask(InputValue value)
    {
        narrationText.text = taunts[Random.Range(0, taunts.Length)];

        Texture2D spriteTexture = maskTextures[Random.Range(0, maskTextures.Length)];

        Rect rect = new Rect(0, 0, spriteTexture.width, spriteTexture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite newSprite = Sprite.Create(spriteTexture, rect, pivot, 100f);

        GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
