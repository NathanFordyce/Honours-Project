using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float speed = 1.2f;
    
    private Transform move;

    [SerializeField] private float sensX = 750f, sensY = 400f;
    private float xRotation, yRotation;

    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MouseControls();
        MovementControls();
        
        if(Input.GetKey(KeyCode.E))
            transform.localPosition += new Vector3(0,2,0) * Time.deltaTime;
        else if(Input.GetKey(KeyCode.Q))
            transform.localPosition -= new Vector3(0,2,0) * Time.deltaTime;

    }

    private void MouseControls()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        // Rotate Camera
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    private void MovementControls()
    {
        float horInput = Input.GetAxis("Horizontal");
        float verInput = Input.GetAxis("Vertical");

        direction = transform.forward * verInput + transform.right * horInput;
        transform.localPosition += direction * Time.deltaTime * speed;
    }
}
