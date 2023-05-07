using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Experimental.Playables;
using Random = UnityEngine.Random;

public class conMovementAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private MiscObjects obstacles;

    private Vector3 startPos;
    
    [Header("Which Environment")]
    [SerializeField] private bool isWalls;
    private void Start()
    {
        startPos = transform.localPosition;
    }
    
    public override void OnEpisodeBegin()
    {
        TrainingProgressText.Episode++;
        
        if (isWalls)    // If wall environment is being used
        {
            obstacles.ResetForEpisode();
            transform.localPosition = new Vector3(-15f, 0f, Random.Range(-7f, 7f));
            targetTransform.localPosition = new Vector3(15f, 0f, Random.Range(-7f, 7f));
        }
        else
        {
            // Give agent and target new starting positions at start of new episode
            // transform.localPosition = new Vector3(Random.Range(-9f, 9f), 0f, Random.Range(-9f, -5f));
            //targetTransform.localPosition = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));

            // Give agent and target new starting position
            transform.localPosition = startPos;
            targetTransform.localPosition = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));

            // GoalRandPos();

            /*int temp = Random.Range(0, 4);
            if(temp == 0)
                targetTransform.localPosition = new Vector3(3f, 0.35f, 0f);
            else if(temp == 1) 
                targetTransform.localPosition = new Vector3(-3f, 0.35f, 0f);
            else if (temp == 2)
                targetTransform.localPosition = new Vector3(0f, 0.35f, 3f);
            else if (temp == 3)
                targetTransform.localPosition = new Vector3(0f, 0.35f, -3f);*/
        }
        
    }
    
    private void FixedUpdate()
    { 
        AddReward(-(1 / MaxStep));
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the agents location and target location
        // sensor.AddObservation(transform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        TrainingProgressText.ScreenText();
        // Store the continuous actions for X and Y positions
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 2.5f; // Agents speed
        
        // Move agent using actions received
        transform.localPosition += new Vector3(moveX, 0, -moveZ) * Time.deltaTime * moveSpeed;   
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> coninuousActions = actionsOut.ContinuousActions;
        coninuousActions[0] = Input.GetAxisRaw("Vertical");
        coninuousActions[1] = Input.GetAxisRaw("Horizontal");

    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal")) // If agent reaches the goal
        {
            TrainingProgressText.Success++;
            
            floorMeshRenderer.material = winMaterial;   // Set floor to pink to show it was successful
            AddReward(1f);  // Reward agent
            // SetReward(1f);  // Reward agent
            EndEpisode();   // End current episode
        }
        
        if (collision.gameObject.CompareTag("Wall")) // If agent goes out of bounds
        {
            TrainingProgressText.Fail++;
            
            floorMeshRenderer.material = loseMaterial;  // Set floor to red to show it failed
            AddReward(-1f); // Punish agent
            // SetReward(-1f); // Punish agent (Used for the initial environment without checkpoints
            EndEpisode();   // End current episode
        }

        if (collision.gameObject.CompareTag("Checkpoint")) // If agent goes out of bounds
        {
            AddReward(0.2f); // Punish agent
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

