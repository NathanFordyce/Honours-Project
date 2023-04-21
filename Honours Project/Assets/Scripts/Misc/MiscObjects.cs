using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MiscObjects : MonoBehaviour
{
    [SerializeField] private Transform[] walls;
    [SerializeField] private GameObject[] checkpoints;
    [SerializeField] private bool isMovement;
    // Start is called before the first frame update
    void Start()
    {
        ResetForEpisode();
    }

    public void ResetForEpisode()
    {
        NewWallPos();

        foreach (var point in checkpoints)
        {
            if (!point.activeSelf)
            {
                print("hello");
                point.SetActive(true);
            }
        }
    }

    private void NewWallPos()
    {
        if (isMovement)
        {
            // Movement environment
            walls[0].localPosition = new Vector3(5f, 1.7f, Random.Range(-10f, 10f));
            walls[1].localPosition = new Vector3(-5f, 1.7f, Random.Range(-10f, 10f));
            if (walls[0].localPosition.z >= walls[1].localPosition.z - 4f && walls[0].localPosition.z <= walls[1].localPosition.z + 4f)      // Movement
            {
                NewWallPos();
            }
        }
        else
        {
            // Shooting Environment
            walls[0].localPosition = new Vector3(Random.Range(-10f, 10f), 1.7f, 5f);
            walls[1].localPosition = new Vector3(Random.Range(-10f, 10f), 1.7f, -5f);
            
            if (walls[0].localPosition.x >= walls[1].localPosition.x - 4f && walls[0].localPosition.x <= walls[1].localPosition.x + 4f)         // Shooting
            {
                NewWallPos();
            }
        }

        // if (walls[0].localPosition.z >= walls[1].localPosition.z - 4f && walls[0].localPosition.z <= walls[1].localPosition.z + 4f)      // Movement
        // //if (walls[0].localPosition.x >= walls[1].localPosition.x - 4f && walls[0].localPosition.x <= walls[1].localPosition.x + 4f)         // Shooting
        // {
        //     NewWallPos();
        // }
    }
    
    
    
    

}
