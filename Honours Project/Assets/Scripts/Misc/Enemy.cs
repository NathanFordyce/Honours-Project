using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int startHealth = 100;
    private int health;
    private void Start()
    {
        health = startHealth;           // Set enemies initial health
    }

    public int GetHit(int damage)       // Reduce enemies health by parameter passed in and return new total health
    {
        health -= damage;
        return health;
    }
    
    private void Update()
    {
        // If enemy is killed reset its health
        if (health <= 0)
        {
            health = startHealth;
        }
    }
}
