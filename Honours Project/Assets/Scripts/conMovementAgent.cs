using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;

public class conMovementAgent : Agent
{
    [Header("Goal Transform")]
    [SerializeField] private Transform targetTransform;
    
    [Header("Floor")]
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    
    [Header("Obstacles")]
    [SerializeField] private MiscObjects obstacles;
    [Header("Stat Text")]
    [SerializeField] private StatsText stats;

    private Vector3 startPos;
    
    [Header("Which Environment")]
    [SerializeField] private bool isWalls;
    
    private void Start()
    {
        startPos = transform.localPosition;             // Store local position at start
    }
    
    public override void OnEpisodeBegin()
    {
        // TrainingProgressText.Episode++;              // Increment total episodes on debug overlay
        stats.Episode++;                                // Increment total episodes on overlay above environment
        
        if (isWalls)                                    // If wall environment is being used
        {
            obstacles.ResetForEpisode();                // Sets walls to new locations and reactivate all checkpoints
            
            // Spawn agent and goal at random locations along the Z axis on opposite sides of environment
            transform.localPosition = new Vector3(-15f, 0f, Random.Range(-7f, 7f));
            targetTransform.localPosition = new Vector3(15f, 0f, Random.Range(-7f, 7f));
        }
        else
        {
            // Give agent and target new starting position
            transform.localPosition = startPos;                                 // Reset agent to starting location
            GoalRandPos();
            
            // Code below was used for the first brain training
            // Goal is randomly spawned in one of the 4 directions
            /*
            int temp = Random.Range(0, 4);
            if(temp == 0)                                                       // Set position to be in positive X direction
                targetTransform.localPosition = new Vector3(3f, 0.35f, 0f);
            else if(temp == 1)                                                  // Set position to be in negative X direction
                targetTransform.localPosition = new Vector3(-3f, 0.35f, 0f);    
            else if (temp == 2)                                                 // Set position to be in positive Z direction
                targetTransform.localPosition = new Vector3(0f, 0.35f, 3f);     
            else if (temp == 3)                                                 // Set position to be in negative Z direction
                targetTransform.localPosition = new Vector3(0f, 0.35f, -3f);
            */
        }
        
    }
    
    private void FixedUpdate()
    { 
        // Update current cumulative reward on overlays
        // TrainingProgressText.Reward = GetCumulativeReward();
        stats.Reward = GetCumulativeReward();

        AddReward(-(1f / MaxStep));     // Punish agent each step
        
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        // TrainingProgressText.ScreenText();           // Displays debugging overlay when simulating
       
        // Store the continuous actions for X and Z positions
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 2.5f;                         // Agents speed
        
        // Move agent in given direction and multiplies by delta time to move smoothly
        transform.localPosition += new Vector3(moveX, 0, -moveZ) * Time.deltaTime * moveSpeed;      
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");         // W = 1 & S = -1 - Control
        continuousActions[1] = Input.GetAxisRaw("Horizontal");       // A = 1 & D = -1 - Control

    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal"))        // If agent reaches the goal
        {
            // TrainingProgressText.Success++;              // Update total success on debug overlay
            stats.Success++;                                // Update total success on overlay above environment
            
            floorMeshRenderer.material = winMaterial;       // Set floor to pink to show it was successful
            AddReward(2f);                          // Reward agent
            EndEpisode();                                   // End current episode
        }
        
        if (collision.gameObject.CompareTag("Wall"))        // If agent goes out of bounds
        {
            // TrainingProgressText.Fail++;                 // Update total fails on debug overlay
            stats.Fail++;                                   // Update total fails on overlay above environment
            
            floorMeshRenderer.material = loseMaterial;      // Set floor to red to show it failed
            AddReward(-1.5f);                       // Punish agent
            EndEpisode();                                   // End current episode
        }

        if (collision.gameObject.CompareTag("Checkpoint"))  // If agent reaches checkpoint
        {
            AddReward(0.25f);                       // Reward agent
            collision.gameObject.SetActive(false);          // Set checkpoint to inactive
        }
    }
    
    private void GoalRandPos()
    {
        targetTransform.localPosition = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
        
        if((targetTransform.localPosition.x < 2.25f && targetTransform.localPosition.x > -2.25f ) || (targetTransform.localPosition.z < 2.25f && targetTransform.localPosition.z > -2.25f )  )
            GoalRandPos();
    }
}

