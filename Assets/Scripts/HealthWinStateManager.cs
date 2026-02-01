using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using DateTimeOffset = System.DateTimeOffset; // Only grab the time tool

public class HealthWinStateManager : MonoBehaviour
{
    // initial health setting.
    public int initialEnemyHealth = 10;
    public int initialPlayerHealth = 10;

    // current health.
    private int enemyHealth = 0;
    private int playerHealth = 0;

    // gradient for player health
    public GameObject playerHealthBarGradient;
    public GameObject enemyHealthBarGradient;

    private float maxScaleX = 0.9914f;

    // Start is called before the first frame update
    void Start()
    {
        // set health based on initial health setting.
        enemyHealth = initialEnemyHealth;
        playerHealth = initialPlayerHealth;

        
    }

    // read only get function for health.
    public int GetEnemyHealth()
    {
        return enemyHealth;
    }

    public int GetPlayerHealth()
    {
        return playerHealth;
    }

    // write function for health
    public int DealDamageToEnemy(int damagePoints)
    {
        if (enemyHealth - damagePoints <= 0)
        {
            enemyHealth = 0;
        } else
        {
            enemyHealth -= damagePoints;
        }

        return enemyHealth;
    }

    public int DealDamageToPlayer(int damagePoints)
    {
        if (playerHealth - damagePoints <= 0)
        {
            playerHealth = 0;
        }
        else
        {
            playerHealth -= damagePoints;
        }

        return playerHealth;
    }

    public int HealPlayer(int healPoints)
    {
        if (playerHealth + healPoints > initialPlayerHealth)
        {
            playerHealth = initialPlayerHealth;
        } else
        {
            playerHealth += healPoints;
        }
        
        return playerHealth;
    }

    public int HealEnemy(int healPoints)
    {
        if (enemyHealth + healPoints > initialEnemyHealth)
        {
            enemyHealth = initialEnemyHealth;
        }
        else
        {
            enemyHealth += healPoints;
        }

        return enemyHealth;
    }


    // Update is called once per frame
    void Update()
    {
        // Prioritise running the kill scene before the win scene.

        if (playerHealth < 1)
        {
            SceneManager.LoadScene("GameOverScene");
        } 
        else
        {
            if (enemyHealth < 1)
            {
                SceneManager.LoadScene("LevelComplete");
            }
        }

        // --- SPRITE HEALTH BAR UPDATE ---

        // 1. Player Sprite Bar
        // Ensure float division and clamp between 0 and 1
        float playerHealthRatio = Mathf.Clamp01((float)playerHealth / (float)initialPlayerHealth);
        float playerNewX = playerHealthRatio * 0.9914f; // Using your exact literal value

        // Access the player bar's current scale, modify ONLY X, and put it back
        Vector3 pScale = playerHealthBarGradient.transform.localScale;
        pScale.x = playerNewX;
        playerHealthBarGradient.transform.localScale = pScale;

        // 2. Enemy Sprite Bar
        float enemyHealthRatio = Mathf.Clamp01((float)enemyHealth / (float)initialEnemyHealth);
        float enemyNewX = enemyHealthRatio * 0.9914f;

        // Access the enemy bar's current scale, modify ONLY X, and put it back
        Vector3 eScale = enemyHealthBarGradient.transform.localScale;
        eScale.x = enemyNewX;
        enemyHealthBarGradient.transform.localScale = eScale;
    }
}
