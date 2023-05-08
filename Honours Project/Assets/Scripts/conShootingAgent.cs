using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class conShootingAgent : Agent
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
        
        Debug.DrawRay(shootPoint.position, direction * 10f, Color.blue, 2f);

        if (Physics.Raycast(shootPoint.position, direction, out var hit, 10f, layerMask))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                if (hit.transform.GetComponent<Enemy>().GetHit(damage) <= 0)    // If enemy has no health
                    EnemyDead();    // Reward agent and end episode
                
            }
            else if (hit.collider.CompareTag("Wall"))   // If shot at wall punish agent
            {
                //floorMeshRenderer.material = loseMaterial;  // Set floor to red to show it failed
                AddReward(-0.2f);               // Punish agent
            }
        }
        else                                            // If shoots and hits nothing
        {
            // floorMeshRenderer.material = loseMaterial;  // Set floor to red to show it failed
            AddReward(-0.2f);                   // Punish agent
        }

        // Set shoot cooldown variables
        shootAvailable = false;
        stepsUntilCanShoot = minStepsBetweenShots;
    }

    private void FixedUpdate()
    {
        TrainingProgressText.Reward = GetCumulativeReward();
        
        AddReward(-(1f / MaxStep)); // Add small punishment each update

        if (!shootAvailable)                // If shoot not available
        {
            stepsUntilCanShoot--;           // Decrease counter

            if (stepsUntilCanShoot <= 0)    // Check if counter is over if so reset availability
                shootAvailable = true;
        }
    }

    public override void Initialize() // Used instead of Start()
    {
        // Stores agents starting location and rotation
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        TrainingProgressText.Episode++;                 // Increment total episodes performed on overlay
        Debug.Log("Episode Begin");

        transform.localPosition = startPos;             // Reset agent to starting position
        // transform.localPosition = new Vector3(-5f, 1.3f, Random.Range(-10f, 10f));       // Reset agent back to starting position
        
        transform.localRotation = startRot;             // Reset agents rotation 
        
        enemyTransform.localPosition = new Vector3(6f, 1.3f, Random.Range(-8f, 8f));    // Set enemy goal to random position along the Z axis
        
        // enemyTransform.localPosition = new Vector3(-4.5f, 1.3f, UnityEngine.Random.Range(-6f, 6f));

        shootAvailable = true;      // Reset shoot check
    }
    
    
    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the agents location
        sensor.AddObservation(transform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Give agent discrete action to shoot if R is pressed
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.R) ? 1 : 0;
        
        // Give agent continuous action to move forward or back if W (1) / S (-1) is pressed
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
       
        // Give agent continuous action to move left and right if A (1) / D (-1) is pressed
        continuousActions[1] = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.Q))        // Give agent continuous action to rotate left when Q is pressed
            continuousActions[2] = -1;
        else if (Input.GetKey(KeyCode.E))   // Give agent continuous action to rotate right when E is pressed
            continuousActions[2] = 1;

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        TrainingProgressText.ScreenText();

        if(actions.DiscreteActions[0] == 1) 
            Shoot();    // Call shoot function
        
        // Store continuous actions to move agent 
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        
        float moveSpeed = 2.5f;
        
        // Move agent in given direction and multiply by delta time to move smoothly
        transform.localPosition += new Vector3(-moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        
        // Store continuous action to rotate agent left and right
        float rot = actions.ContinuousActions[2];
        transform.Rotate(0f,rot,0f);    // Rotate agent in given direction
        
    }

    public void EnemyDead()
    {
        floorMeshRenderer.material = winMaterial;   // Set floor to pink to show it was successful
        TrainingProgressText.Success++;             // Increment total successes on overlay
        AddReward(1f);                      // Reward agent of killing enemy
        EndEpisode();                               // End current episode
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy")      // If agent collides with environment or Enemy
        {
            floorMeshRenderer.material = loseMaterial;  // Set floor to red to show agent failed
            TrainingProgressText.Fail++;                // Increment total fails on overlay
            AddReward(-1f);                     // Punish agent
            EndEpisode();                               // End current episode
        }
    }
}
