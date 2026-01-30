using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // Must have this!
using UnityEngine.SceneManagement;
using DateTimeOffset = System.DateTimeOffset; // Only grab the time tool


public class EnemyMaskController : MonoBehaviour
{
    // the text object.
    public TMP_Text narrationText;
    public TMP_Text debugText;

    public string[] taunts;
    public Texture2D[] maskTextures;
    public SpriteRenderer maskSprite;

    public int health = 10;

    public int currentTaunt = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnDebugSwitchMask(InputValue value)
    {
        health--;
        currentTaunt = Random.Range(0, taunts.Length);

        Texture2D spriteTexture = maskTextures[Random.Range(0, maskTextures.Length)];

        Rect rect = new Rect(0, 0, spriteTexture.width, spriteTexture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite newSprite = Sprite.Create(spriteTexture, rect, pivot, 100f);

        maskSprite.sprite = newSprite;

        if (health < 1)
        {
            SceneManager.LoadScene("LevelComplete");
        }
    }

    private int prevTaunt = 1;
    private int printedChars = 0;
    private long lastTextUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public AudioSource typingSound;
    public AudioClip typingSoundClip;

    // Update is called once per frame
    void Update()
    {
        debugText.text = "Health: " + health.ToString();
        if (prevTaunt != currentTaunt)
        {
            // taunt has changed reset
            narrationText.text = "";
            printedChars = 0;
            prevTaunt = currentTaunt;
        }

        // check ifw e have printed all chars
        if (taunts[currentTaunt].Length != narrationText.text.Length)
        {
            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastTextUpdate >= 15)
            {
                narrationText.text = narrationText.text + taunts[currentTaunt][printedChars];
                printedChars++;
                Debug.Log(printedChars);
                lastTextUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                typingSound.PlayOneShot(typingSoundClip, 0.5f);
            }
        }
    }
}
