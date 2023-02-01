using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform orientation;

    [Header("Drag")]
    [SerializeField] private float groundDrag;

    [Header("Gun")]
    [SerializeField] private Gun weapon;


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.drag = groundDrag;
    }

    private void Update()
    {
        MyInput();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        MoveAgent();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MoveAgent()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void CheckAmmo()
    {
        //if(weapon.getammo)
    }


    public void LookAt(Vector3 target)
    {
        Vector3 newTarget = new Vector3(target.x, transform.position.y, target.z);
        transform.LookAt(target);
    }

    public void ResetLook()
    {
        transform.rotation = Quaternion.identity;
    }
}
