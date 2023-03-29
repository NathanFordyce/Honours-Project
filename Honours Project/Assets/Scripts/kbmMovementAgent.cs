using System;
using System.Collections;
using System.Collections.Generic;
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

    private Vector3 startPos;
    private void Start()
    {
        startPos = transform.localPosition;
    }
    
    public override void OnEpisodeBegin()
    {
        TrainingProgressText.Episode++;
        obstacles.ResetForEpisode();
        // Give agent and target new starting positions at start of new episode
        // transform.localPosition = new Vector3(Random.Range(-9f, 9f), 0f, Random.Range(-9f, -5f));
        // transform.localPosition = new Vector3(Random.Range(-9f, 9f), 0f, -7f);
        // targetTransform.localPosition = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
        // targetTransform.localPosition = new Vector3(Random.Range(-5f, 5f), 0f, 7f);
        
        // Environment with walls spawn points
        transform.localPosition = new Vector3(Random.Range(-7f, 7f), 0f, -15f);
        targetTransform.localPosition = new Vector3(Random.Range(-7f, 7f), 0f, 15f);

        
        
        //transform.localPosition = startPos;

        // int temp = Random.Range(0, 3);
        //
        // if(temp == 0)
        //     targetTransform.localPosition = new Vector3(3f, 0.35f, 0f);
        // else if(temp == 1) 
        //     targetTransform.localPosition = new Vector3(-3f, 0.35f, 0f);
        // else if (temp == 2)
        //     targetTransform.localPosition = new Vector3(0f, 0.35f, 3f);
        // else if (temp == 3)
        //     targetTransform.localPosition = new Vector3(0f, 0.35f, -3f);
        
    }

    private void FixedUpdate()
    {
        SetReward(-(1 / MaxStep));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the agents location and target location
        // sensor.AddObservation(transform.localPosition);
        // sensor.AddObservation(targetTransform.localPosition);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        TrainingProgressText.ScreenText();
        // Store the continuous actions for X and Y positions
        float moveX = 0;
        float moveZ = 0;
        
        float moveSpeed = 2.5f; // Agents speed

        // Use corresponding value to move in one of 8 directions
        switch (actions.DiscreteActions[0])
        {
            case 1:
                moveX = 1;
                break;
            case 2:
                moveX = -1;
                break;
            case 3:
                moveZ = 1;
                break;
            case 4:
                moveZ = -1;
                break;
            case 5:
                moveX = 1;
                moveZ = 1;
                break;
            case 6:
                moveX = -1;
                moveZ = -1;
                break;
            case 7:
                moveX = -1;
                moveZ = 1;
                break;
            case 8:
                moveX = 1;
                moveZ = -1;
                break;
        }
        

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;   // Move agent using actions received
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W))
            discreteActions[0] = 1;
        else if (Input.GetKey(KeyCode.S))
            discreteActions[0] = 2;
        else if (Input.GetKey(KeyCode.D))
            discreteActions[0] = 3;
        else if (Input.GetKey(KeyCode.A))
            discreteActions[0] = 4;
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
        if (collision.gameObject.CompareTag("Goal")) // If agent reaches the goal
        {
            TrainingProgressText.Success++;
            SetReward(1f);  // Reward agent
            floorMeshRenderer.material = winMaterial;   // Set floor to pink to show it was successful
            EndEpisode();   // End current episode
        }
        if (collision.gameObject.CompareTag("Wall")) // If agent goes out of bounds
        {
            TrainingProgressText.Fail++;
            
            transform.localPosition = startPos;
            SetReward(-1f); // Punish agent
            floorMeshRenderer.material = loseMaterial;  // Set floor to red to show it failed
            EndEpisode();   // End current episode
        }

        if (collision.gameObject.CompareTag("Checkpoint")) // If agent goes out of bounds
        {
            SetReward(0.2f); // Punish agent
            collision.gameObject.SetActive(false);
        }
    }
}
