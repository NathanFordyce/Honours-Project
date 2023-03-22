using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor;
using UnityEngine.Analytics;

public class kbmShootingAgent : Agent
{
    [Header("Shooting")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private int minStepsBetweenShots = 50;
    [SerializeField] private int damage = 100;
    [SerializeField] private GameObject bulletPrefab;
   
    [Header("Enemy")]
    [SerializeField] private Transform enemyTransform;
    
    [Header("Floor")]
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;


    private bool shootAvailable = true;
    private int stepsUntilCanShoot = 0;

    private Vector3 startPos;
    private Quaternion startRot;
    private Rigidbody rb;

    private bool switchSide = false;
    
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

        Debug.DrawRay(shootPoint.position, direction * 10f, Color.blue, 2f);

        if (Physics.Raycast(shootPoint.position, direction, out var hit, 15f, layerMask))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                if (hit.transform.GetComponent<Enemy>().GetHit(damage) <= 0)    // If enemy has no health
                    EnemyDead();    // Reward agent and end episode
                
            }
            else if (hit.collider.CompareTag("Wall"))   // If hit wall
            {
                print("Hello");
                TrainingProgressText.Fail++;
                floorMeshRenderer.material = loseMaterial;  // Set floor to red to show it failed
                AddReward(-0.2f);      // Punish agent
            }
        }
        else
        {
            TrainingProgressText.Fail++;
            floorMeshRenderer.material = loseMaterial;  // Set floor to red to show it failed
            AddReward(-0.2f);      // Punish agent
        }

        // Set shoot cooldown variables
        shootAvailable = false;
        stepsUntilCanShoot = minStepsBetweenShots;
    }

    private void FixedUpdate()
    {
        TrainingProgressText.Reward = GetCumulativeReward();
        
        SetReward(-(1 / MaxStep));

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
        startRot = transform.localRotation; // Get players starting rotation
        rb = GetComponent<Rigidbody>();     // Get agents rigid body component
    }

    public override void OnEpisodeBegin()
    {
        TrainingProgressText.Episode++;
        
        
        Debug.Log("Episode Begin");
        
        // transform.localPosition = startPos;     // Reset agent back to starting position
        // transform.localRotation = startRot;
        //enemyTransform.localPosition = new Vector3(-4.5f, 1.3f, UnityEngine.Random.Range(-6f, 6f));
        
        // Move enemy object to new position
        if(!switchSide)
            enemyTransform.localPosition = new Vector3(-4.5f, 1.3f, UnityEngine.Random.Range(-6f, 6f));
        else
            enemyTransform.localPosition = new Vector3(4.5f, 1.3f, UnityEngine.Random.Range(-6f, 6f));

        //rb.velocity = Vector3.zero; // Stop agent from moving
        shootAvailable = true;      // Reset shoot check
        switchSide = !switchSide;
    }
    
    
    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the agents location and enemy location
        sensor.AddObservation(transform.localPosition);
        // sensor.AddObservation(enemyTransform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Give agent discrete action if E is pressed
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[1] = Input.GetKey(KeyCode.R) ? 1 : 0;
        
        // Give agent continuous action if A/D is pressed
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        if (Input.GetKey(KeyCode.Q))
            continuousActions[0] = -1;
        else if (Input.GetKey(KeyCode.E))
            continuousActions[0] = 1;
        
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
        
        if(actions.DiscreteActions[1] == 1) 
            Shoot();    // Call shoot function
        
        float rot = actions.ContinuousActions[0];
        transform.Rotate(0f,rot,0f);
        
        
    }

    public void EnemyDead()
    {
        floorMeshRenderer.material = winMaterial;  // Set floor to red to show it failed
        TrainingProgressText.Success++;
        AddReward(1f);  // Reward agent of killing enemy
        EndEpisode();           // End current episode
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy") // If agent goes out of bounds
        {
            floorMeshRenderer.material = loseMaterial;  // Set floor to red to show it failed
            
            transform.localPosition = startPos;     // Reset agent back to starting position
            transform.localRotation = startRot;
            
            TrainingProgressText.Fail++;
            SetReward(-1f); // Punish agent
            EndEpisode();   // End current episode
        }
    }
}
