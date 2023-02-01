using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ShootingAgent : Agent
{
    [Header("Shooting")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private int minStepsBetweenShots = 50;
    [SerializeField] private int damage = 100;
    [SerializeField] private GameObject bulletPrefab;
   
    [Header("Enemy")]
    [SerializeField] private Transform enemyTransform;


    private bool shootAvailable = true;
    private int stepsUntilCanShoot = 0;

    private Vector3 startPos;
    private Rigidbody rb;
    
    
    private void Shoot()
    {
        if(!shootAvailable)
            return;
    
        // Spawn visual bullet
        var bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = shootPoint.forward * 10; // Move bullet forward
        
        // Perform raycast to check what agent has hit
        var layerMask = 1 << LayerMask.NameToLayer("Enemy");
        var direction = transform.forward;
        
        Debug.Log("Shooting");

        Debug.DrawRay(shootPoint.position, direction * 15f, Color.red, 2f);

        if (Physics.Raycast(shootPoint.position, direction, out var hit, 15f, layerMask))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                if (hit.transform.GetComponent<Enemy>().GetHit(damage) <= 0)    // If enemy has no health
                    EnemyDead();    // Reward agent and end episode
                
            }
            else if (hit.collider.CompareTag("Wall"))   // If hit wall
            {
                AddReward(-0.02f);      // Punish agent
            }


        }

        // Set shoot cooldown variables
        shootAvailable = false;
        stepsUntilCanShoot = minStepsBetweenShots;
    }

    private void FixedUpdate()
    {
        if (!shootAvailable)    // If shoot not available
        {
            stepsUntilCanShoot--;       // Decrease counter

            if (stepsUntilCanShoot <= 0)    // Check if counter is over
                shootAvailable = true;
        }
        
    }

    public override void Initialize() // Used instead of Start()
    {
        startPos = transform.localPosition; // Get players starting position
        rb = GetComponent<Rigidbody>();     // Get agents rigid body component
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Episode Begin");
        transform.localPosition = startPos;     // Reset agent back to starting position
        // Move enemy object to new position
        enemyTransform.localPosition = new Vector3(-4.5f, 1.3f, UnityEngine.Random.Range(-7f, 7f));
        rb.velocity = Vector3.zero; // Stop agent from moving
        shootAvailable = true;      // Reset shoot check
    }
    
    
    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the agents location and enemy location
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(enemyTransform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Give agent discrete action if E is pressed
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.E) ? 1 : 0;
        
        // Give agent continuous action if A/D is pressed
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if(actions.DiscreteActions[0] == 1) 
            Shoot();    // Call shoot function
        
        float moveZ = actions.ContinuousActions[0];
        
        float moveSpeed = 2.5f;
        // Move the agent in the direction given by continuous action
        transform.localPosition += new Vector3(0, 0, moveZ) * Time.deltaTime * moveSpeed;  

    }

    public void EnemyDead()
    {
        AddReward(1f);  // Reward agent of killing enemy
        EndEpisode();           // End current episode
    }
}
