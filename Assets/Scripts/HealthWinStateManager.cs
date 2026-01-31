using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HealthWinStateManager : MonoBehaviour
{
    // initial health setting.
    public int initialHealth = 10;

    // current health.
    private int health = 0;

    // Start is called before the first frame update
    void Start()
    {
        // set health based on initial health setting.
        health = initialHealth;
    }

    // read only get function for health.
    public int GetHealth()
    {
        return health;
    }

    // write function for health
    public int DealDamage(int damagePoints)
    {
        if (health - damagePoints <= 0)
        {
            health = 0;
        } else
        {
            health -= damagePoints;
        }

        return health;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < 1)
        {
            SceneManager.LoadScene("LevelComplete");
        }
    }
}
