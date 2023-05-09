using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class kbmMovementAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private MiscObjects obstacles;
    
    [Header("Stat Text")]
    [SerializeField] private StatsText stats;

    [Header("Which Environment")]
    [SerializeField] private bool isWalls;

    private Vector3 startPos;
    private void Start()
    {
        startPos = transform.localPosition;         // Store agents initial position
    }
    
    public override void OnEpisodeBegin()
    {
        // TrainingProgressText.Episode++;              // Increment total episodes on debug overlay
        stats.Episode++;                                // Increment total episodes on overlay above environment

        if (isWalls)                                    // If using wall environment
        {
            obstacles.ResetForEpisode();                // Sets walls to new locations and reactivate all checkpoints
            
            // Spawn agent and goal at random locations along the Z axis on opposite sides of environment
            transform.localPosition = new Vector3(-15f, 0f, Random.Range(-7f, 7f));
            targetTransform.localPosition = new Vector3(15f, 0f, Random.Range(-7f, 7f));
        }
        else
        {
            // Give agent and target new starting position
            transform.localPosition = startPos;                                 // Reset agent to starting position
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
        
        float moveX = 0;
        float moveZ = 0;
        
        float moveSpeed = 2.5f; // Agents speed

        // Use corresponding value to move in one of 8 directions
        switch (actions.DiscreteActions[0])
        {
            case 1:             // Move agent forward
                moveX = 1;
                transform.localPosition += new Vector3(moveX, 0, 0) * Time.deltaTime * moveSpeed;
                break;
            case 2:             // Move agent back
                moveX = -1;
                transform.localPosition += new Vector3(moveX, 0, 0) * Time.deltaTime * moveSpeed;

                break;
            case 3:             // Move agent left
                moveZ = 1;
                transform.localPosition += new Vector3(0, 0, moveZ) * Time.deltaTime * moveSpeed;
                break;
            case 4:             // Move agent right
                moveZ = -1;
                transform.localPosition += new Vector3(0, 0, moveZ) * Time.deltaTime * moveSpeed;
                break;
            case 5:             // Move agent forward and to left
                moveX = 1;
                moveZ = 1;
                transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
                break;
            case 6:             // Move agent back and to right
                moveX = -1;
                moveZ = -1;
                transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
                break;
            case 7:             // Move agent back and to left
                moveX = -1;
                moveZ = 1;
                transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
                break;
            case 8:             // Move agent forward and to right
                moveX = 1;
                moveZ = -1;
                transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
                break;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        // WASD Controls to move agent when in heuristic mode
        if (Input.GetKey(KeyCode.W))
            discreteActions[0] = 1;
        else if (Input.GetKey(KeyCode.S))
            discreteActions[0] = 2;
        else if (Input.GetKey(KeyCode.A))
            discreteActions[0] = 3;
        else if (Input.GetKey(KeyCode.D))
            discreteActions[0] = 4;
        
        // Controls to move agent in diagonal directions
        else if (Input.GetKey(KeyCode.P))
            discreteActions[0] = 5;
        else if (Input.GetKey(KeyCode.O))
            discreteActions[0] = 6;
        else if (Input.GetKey(KeyCode.I))
            discreteActions[0] = 7;
        else if (Input.GetKey(KeyCode.U))
            discreteActions[0] = 8;
        else if (Input.GetKey(KeyCode.Space))
            discreteActions[0] = 0;


    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal"))        // If agent reaches the goal
        {
            // TrainingProgressText.Success++;              // Increment total successes on debug overlay
            stats.Success++;                                // Increment total successes on overlay above environment
            
            floorMeshRenderer.material = winMaterial;       // Set floor to pink to show agent was successful
            AddReward(2f);                          // Reward agent
            EndEpisode();                                   // End current episode
        }
        
        if (collision.gameObject.CompareTag("Wall"))        // If agent goes out of bounds
        {
            // TrainingProgressText.Fail++;                 // Increment total fails on debug overlay
            stats.Fail++;                                   // Increment total fails on overlay above environment
            
            floorMeshRenderer.material = loseMaterial;      // Set floor to red to show agent failed
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
