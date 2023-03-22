using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float life = 3;

    private Gun gunREF;

    private void Awake()
    {
        Destroy(gameObject, life);
    }

    //private void OnTriggerEnter(Collider other)
    //{

    //    if (other.tag == "Ignore")
    //    {
    //        return;
    //    }
    //    gunREF.AddToList(other.tag);
    //    gameObject.SetActive(false);
        

    //}

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Colliding");
        // if (collision.gameObject.tag == "Ignore")
        // {
        //     return;
        // }
        // gunREF.AddToList(collision.gameObject.tag);
        gameObject.SetActive(false);
    }

    public void SetGunRef(Gun gun)
    {
        gunREF = gun;
    }
    
}
