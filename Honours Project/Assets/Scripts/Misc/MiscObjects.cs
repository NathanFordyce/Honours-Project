using UnityEngine;

public class MiscObjects : MonoBehaviour
{
    [SerializeField] private Transform[] walls;
    [SerializeField] private GameObject[] checkpoints;

    // Start is called before the first frame update
    void Start()
    {
        ResetForEpisode();          // Reset walls at start
    }

    public void ResetForEpisode()
    {
        NewWallPos();               // Set walls to new position

        // Loops through each checkpoint to see if inactive
        foreach (var point in checkpoints)
        {
            // If checkpoint is inactive then set back to active
            if (!point.activeSelf)
            {
                point.SetActive(true);
            }
        }
    }

    private void NewWallPos()
    {
        // Sets both walls to random X values between -10 and 10
        walls[0].localPosition = new Vector3(Random.Range(-10f, 10f), 1.7f, 5f);
        walls[1].localPosition = new Vector3(Random.Range(-10f, 10f), 1.7f, -5f);
        
        // Checks if walls are too close to one another and recalls this function if they are
        if (walls[0].localPosition.x >= walls[1].localPosition.x - 5f && walls[0].localPosition.x <= walls[1].localPosition.x + 5f)
            NewWallPos();
    }
    
    
    
    

}
