using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DateTimeOffset = System.DateTimeOffset; // Only grab the time tool

public class NarrationController : MonoBehaviour
{
    /// ------ PUBLIC VARIABLES ------ ///
    
    // generic taunts for game start.
    public string[] genericTaunts;

    // taunts for when enemy is at half health
    public string[] halfHealthTaunts;

    // taunts for when enemy is at critical health (<10%?)
    public string[] criticalTaunts;

    // typing sound.
    public AudioSource typingSound;

    // link to the win state manager
    public HealthWinStateManager healthWinStateManager;

    /// ------ PRIVATE VARIABLES ------ ///
    
    // currently selected taunt array
    private string[] taunts;

    // variable store for current taunt.
    private int currentTaunt = 0;

    // set to TRUE when taunt has been changed since last cycle otherwise FALSE.
    private bool tauntChangedSinceLastCycle = true;

    // last unix time stamp in ms since text update - used for calculating type delay.
    private long lastTextUpdate = 0;

    // number of printed characters for typing. used to select next character for display.
    private int printedChars = 0;

    // these are letters that the typing renderer should add more delay to for suspense.
    private char[] slowLetters = { '.', ',', '!', '?' };


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private int prevHealth;
    // Update is called once per frame
    void Update()
    {
        int health = healthWinStateManager.GetHealth();

        // get text from narration object.
        TMP_Text narrationText = GetComponent<TMP_Text>();

        if (health != prevHealth)
        {
            // temp health example
            if (health <= healthWinStateManager.initialHealth / 2)
            {
                if (health <= healthWinStateManager.initialHealth / 4)
                {
                    // player at critical health!
                    taunts = criticalTaunts;
                    currentTaunt = Random.Range(0, taunts.Length);
                }
                else
                {
                    // player at half health!
                    taunts = halfHealthTaunts;
                    currentTaunt = Random.Range(0, taunts.Length);
                }
            } else
            {
                // use generic taunts.
                taunts = genericTaunts;
                currentTaunt = Random.Range(0, taunts.Length);
            }

            tauntChangedSinceLastCycle = true;
            prevHealth = health;
        }


        if (tauntChangedSinceLastCycle)
        {
            // taunt has changed reset
            narrationText.text = "";
            printedChars = 0;
            tauntChangedSinceLastCycle = false;
        }

        // check if whave printed all chars
        if (taunts[currentTaunt].Length != narrationText.text.Length)
        {
            int delay = 20;

            if (slowLetters.Contains(taunts[currentTaunt][printedChars]))
            {
                delay += 150;
            }

            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastTextUpdate >= delay)
            {
                narrationText.text = narrationText.text + taunts[currentTaunt][printedChars];
                printedChars++;
                lastTextUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                typingSound.pitch = Random.Range(0.8f, 1f);
                typingSound.PlayOneShot(typingSound.clip, 0.5f);
            }
        }
    }
}
