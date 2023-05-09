using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float life = 3;
    
    private void Awake()
    {
        Destroy(gameObject, life);      // Destroy bullet after 3 seconds
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);            // Destroy bullet if it collides with anything
    }
}
