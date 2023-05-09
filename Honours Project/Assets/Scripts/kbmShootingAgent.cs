using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

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

    [Header("Demo Scene")]
    [SerializeField] private StatsText stats;

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
            else if (hit.collider.CompareTag("Wall"))   // If agent shot at wall
            {
                //floorMeshRenderer.material = loseMaterial;  // Set floor to red to show it failed
                AddReward(-0.25f);               // Punish agent
            }
        }
        else                                            // If agent shoots and hits nothing
        {
            //floorMeshRenderer.material = loseMaterial;// Set floor to red to show it failed
            AddReward(-0.25f);                   // Punish agent
        }

        // Set shoot cooldown variables
        shootAvailable = false;
        stepsUntilCanShoot = minStepsBetweenShots;
    }

    private void FixedUpdate()
    {
        // TrainingProgressText.Reward = GetCumulativeReward();
        stats.Reward = GetCumulativeReward();

        AddReward(-(1f / MaxStep)); // Add small punishment each update

        if (!shootAvailable)                // If shoot not available
        {
            stepsUntilCanShoot--;           // Decrease counter

            if (stepsUntilCanShoot <= 0)    // Check if counter is over if so reset availability
                shootAvailable = true;
        }
        
        float angle = Vector3.Angle(transform.forward, enemyTransform.position - transform.position);       // Calculates angle between enemy goal and agent in front of it 
        if (Mathf.Abs(angle) < 30f)
        {
            if(Vector3.Distance(transform.position, enemyTransform.position) < 6f)      // If within vision and close proximity
                transform.LookAt(enemyTransform);                                           // Make agent look directly at enemy goal
        }
    }

    public override void Initialize() // Used instead of Start()
    {
        // Store agents local start location and rotation
        startPos = transform.localPosition;
        startRot = transform.localRotation; 
    }

    public override void OnEpisodeBegin()
    {
        // TrainingProgressText.Episode++;             // Increment total episodes performed on overlay
        stats.Episode++;             // Increment total episodes performed on overlay
        Debug.Log("Episode Begin");

        // Set agents start location and rotation
        transform.localPosition = startPos;         // Reset agent back to starting position
        // transform.localPosition = new Vector3(-8f, 1.3f, Random.Range(-10f, 10f));     // Reset agent back to starting position

        transform.localRotation = startRot;         // Reset agents rotation

        // Set goal to new random position
        enemyTransform.localPosition = new Vector3(6f, 1.3f, Random.Range(-8f, 8f));
        
        // enemyTransform.localPosition = new Vector3(Random.Range(6f, 9f), 1.3f, Random.Range(-10f, 10f));
        
        shootAvailable = true;      // Reset shoot check
    }
    
    
    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the agents location
        sensor.AddObservation(transform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Give agent discrete action if R is pressed to shoot
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[1] = Input.GetKey(KeyCode.R) ? 1 : 0;
        
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        if (Input.GetKey(KeyCode.Q))
            continuousActions[0] = -1;
        else if (Input.GetKey(KeyCode.E))
            continuousActions[0] = 1;
        
        // WASD Controls to move agent when in heuristic mode
        if (Input.GetKey(KeyCode.W))            // Forward Movement
            discreteActions[0] = 1;
        else if (Input.GetKey(KeyCode.S))       // Backward Movement
            discreteActions[0] = 2;
        else if (Input.GetKey(KeyCode.A))       // Left Movement
            discreteActions[0] = 3;
        else if (Input.GetKey(KeyCode.D))       // Right Movement
            discreteActions[0] = 4;
        
        // Controls to move agent in diagonal directions
        else if (Input.GetKey(KeyCode.U))       // Forward to Left Diagonal Movement
            discreteActions[0] = 5;
        else if (Input.GetKey(KeyCode.O))       // Backward to Right Diagonal Movement
            discreteActions[0] = 6;
        else if (Input.GetKey(KeyCode.P))       // Backward to Left Diagonal Movement
            discreteActions[0] = 7;
        else if (Input.GetKey(KeyCode.I))       // Forward to Right Diagonal Movement
            discreteActions[0] = 8;
        else if (Input.GetKey(KeyCode.Space))   // No movement
            discreteActions[0] = 0;

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // TrainingProgressText.ScreenText();
        
        // Store the continuous actions for X and Y positions
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
        
        if(actions.DiscreteActions[1] == 1) 
            Shoot();    // Call shoot function
        
        // Store continuous action to rotate agent left and right
        float rot = actions.ContinuousActions[0];
        transform.Rotate(0f,rot,0f);    // Rotate agent left or right
        
        
    }

    public void EnemyDead()
    {
        floorMeshRenderer.material = winMaterial;   // Set floor to pink to show agent was successful
        // TrainingProgressText.Success++;          // Increment total success on overlay
        stats.Success++;                            // Increment total success on overlay above environment
        AddReward(1.5f);                    // Reward agent of killing enemy
        EndEpisode();                               // End current episode
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy") // If agent goes out of bounds
        {
            floorMeshRenderer.material = loseMaterial;  // Set floor to red to show agent failed
            // TrainingProgressText.Fail++;             // Increment total fails on overlay
            stats.Fail++;                               // Increment total fails on overlay above environment
            AddReward(-2f);                     // Punish agent
            EndEpisode();                               // End current episode
        }
    }
}
