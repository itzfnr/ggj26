using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // Must have this!
using UnityEngine.SceneManagement;
using DateTimeOffset = System.DateTimeOffset; // Only grab the time tool


public class EnemyMaskController : MonoBehaviour
{
    // the text object.
    public TMP_Text debugText;

    public Texture2D[] maskTextures;
    public SpriteRenderer maskSprite;

    // link to the win state manager
    public HealthWinStateManager healthWinStateManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnDebugSwitchMask(InputValue value)
    {
        healthWinStateManager.DealDamage(1);

        Texture2D spriteTexture = maskTextures[Random.Range(0, maskTextures.Length)];

        Rect rect = new Rect(0, 0, spriteTexture.width, spriteTexture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite newSprite = Sprite.Create(spriteTexture, rect, pivot, 100f);

        maskSprite.sprite = newSprite;

      
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = "Health: " + healthWinStateManager.GetHealth().ToString();
    }
}
