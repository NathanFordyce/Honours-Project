using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
    //[SerializeField] private MoveToGoalAgent AgentREF;
    [SerializeField] private AgentMovement AgentREF;
    private Transform target;
    private bool look = false; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            look = true;
            target = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            look = false;
            target = null;

            AgentREF.ResetLook();
        }
    }

    private void Update()
    {
        transform.position = AgentREF.transform.position;
        transform.rotation = AgentREF.transform.rotation;

        if (look)
            AgentREF.LookAt(target.position);
    }


}
