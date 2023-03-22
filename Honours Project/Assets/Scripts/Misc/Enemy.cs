using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Enemy : MonoBehaviour
{

    private int startHealth = 100;
    private int health;

    private void Start()
    {
        health = startHealth;
    }

    public int GetHit(int damage)
    {
        health -= damage;
        return health;
    }
    
    
    private void Respawn()
    {
        health = startHealth;
    }


    private void Update()
    {
        if (health <= 0)
        {
            Respawn();
        }
    }

    public int GetHealth()
    {
        return health;
    }
}
