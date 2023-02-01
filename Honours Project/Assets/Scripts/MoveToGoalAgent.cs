using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    
    public override void OnEpisodeBegin()
    {
        // Give agent and target new starting positions at start of new episode
        transform.localPosition = new Vector3(Random.Range(-9f, 9f), 0f, Random.Range(-9f, -5f));
        targetTransform.localPosition = new Vector3(Random.Range(-9f, 9f), 0f, Random.Range(5f, 9f));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the agents location and target location
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Store the continuous actions for X and Y positions
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 2.5f; // Agents speed
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;   // Move agent using actions received
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> coninuousActions = actionsOut.ContinuousActions;
        coninuousActions[0] = Input.GetAxisRaw("Horizontal");
        coninuousActions[1] = Input.GetAxisRaw("Vertical");

    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Goal") // If agent reaches the goal
        {
            SetReward(1f);  // Reward agent
            floorMeshRenderer.material = winMaterial;   // Set floor to pink to show it was successful
            EndEpisode();   // End current episode
        }
        if (collision.gameObject.tag == "Wall") // If agent goes out of bounds
        {
            SetReward(-1f); // Punish agent
            floorMeshRenderer.material = loseMaterial;  // Set floor to red to show it failed
            EndEpisode();   // End current episode
        }
    }
}

