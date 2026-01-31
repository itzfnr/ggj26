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
        playerHealth += healPoints;
        return playerHealth;
    }

    public int HealEnemy(int healPoints)
    {
        enemyHealth += healPoints;
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
    }
}
