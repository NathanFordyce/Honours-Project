using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MiscObjects : MonoBehaviour
{
    [SerializeField] private Transform[] walls;
    [SerializeField] private GameObject[] checkpoints;
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
                point.SetActive(true);
            }
        }
    }

    private void NewWallPos()
    {
        walls[0].localPosition = new Vector3(5f, 1.7f, Random.Range(-10f, 10f));
        walls[1].localPosition = new Vector3(-5f, 1.7f, Random.Range(-10f, 10f));

        if (walls[0].localPosition.z >= walls[1].localPosition.z - 4f && walls[0].localPosition.z <= walls[1].localPosition.z + 4f)
        {
            print("another new pos");
            NewWallPos();
        }
    }
    
    
    
    

}
