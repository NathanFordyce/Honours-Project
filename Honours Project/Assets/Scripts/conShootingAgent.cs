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
        
        Debug.DrawRay(shootPoint.position, direction * 10f, Color.blue, 2f);

        if (Physics.Raycast(shootPoint.position, direction, out var hit, 10f, layerMask))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                if (hit.transform.GetComponent<Enemy>().GetHit(damage) <= 0)    // If enemy has no health
                    EnemyDead();    // Reward agent and end episode
                
            }
            else if (hit.collider.CompareTag("Wall"))   // If hit wall
            {
                // TrainingProgressText.Fail++;
                //floorMeshRenderer.material = loseMaterial;  // Set floor to red to show it failed
                AddReward(-0.2f);      // Punish agent
                // EndEpisode();   // End current episode
            }
        }
        else
        {
            // TrainingProgressText.Fail++;
            // floorMeshRenderer.material = loseMaterial;  // Set floor to red to show it failed
            AddReward(-0.2f);      // Punish agent
            //EndEpisode();   // End current episode
        }

        // Set shoot cooldown variables
        shootAvailable = false;
        stepsUntilCanShoot = minStepsBetweenShots;
    }

    private void FixedUpdate()
    {
        TrainingProgressText.Reward = GetCumulativeReward();
        
        AddReward(-(1f / MaxStep));

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
        startRot = transform.localRotation; // Get players starting position
        rb = GetComponent<Rigidbody>();     // Get agents rigid body component
    }

    public override void OnEpisodeBegin()
    {
        TrainingProgressText.Episode++;
        Debug.Log("Episode Begin");
        
        transform.localPosition = startPos;     // Reset agent back to starting position
        transform.localRotation = startRot;
        
        enemyTransform.localPosition = new Vector3(6f, 1.3f, Random.Range(-8f, 8f));
        
        // enemyTransform.localPosition = new Vector3(-4.5f, 1.3f, UnityEngine.Random.Range(-6f, 6f));

        // Move enemy object to new position
        // if(!switchSide)
        //     enemyTransform.localPosition = new Vector3(-4.5f, 1.3f, UnityEngine.Random.Range(-6f, 6f));
        // else
        //     enemyTransform.localPosition = new Vector3(4.5f, 1.3f, UnityEngine.Random.Range(-6f, 6f));

        shootAvailable = true;      // Reset shoot check
    }
    
    
    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the agents location and enemy location
        sensor.AddObservation(transform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Give agent discrete action if E is pressed
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.R) ? 1 : 0;
        
        // Give agent continuous action if A/D is pressed
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
       
        // Give agent continuous action if A/D is pressed
        continuousActions[1] = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.Q))
            continuousActions[2] = -1;
        else if (Input.GetKey(KeyCode.E))
            continuousActions[2] = 1;

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if(actions.DiscreteActions[0] == 1) 
            Shoot();    // Call shoot function
        
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        
        float moveSpeed = 2.5f;
        // Move the agent in the direction given by continuous action
        transform.localPosition += new Vector3(-moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        
        float rot = actions.ContinuousActions[2];
        transform.Rotate(0f,rot,0f);
        
        TrainingProgressText.ScreenText();
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
            AddReward(-1f); // Punish agent
            EndEpisode();   // End current episode
        }
    }
}
