using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float sensX = 375f, sensY = 200f;
    [SerializeField] private Text overlay;
    
    private float xRotation, yRotation;
    private Transform move;

    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        // Lock cursor and set to be invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MouseControls();
        MovementControls();
        
        
        if(Input.GetKey(KeyCode.E))                                                     // If E is pressed move camera up
            transform.localPosition += new Vector3(0,speed,0) * Time.deltaTime;
        else if(Input.GetKey(KeyCode.Q))                                                // If Q is pressed move camera down
            transform.localPosition -= new Vector3(0,speed,0) * Time.deltaTime;
        else if(Input.GetKeyDown(KeyCode.Escape))                                       // If escape is pressed quit
            Application.Quit();

        // If 1 or 2 is pressed change speed
        if (Input.GetKeyDown(KeyCode.Alpha1))
            speed -= 5;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            speed += 5;
        // If 3 or 4 is pressed change X sensitivity 
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            sensX -= 10;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            sensX += 10;
        // If 5 or 6 is pressed change Y sensitivity 
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            sensY -= 10;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            sensY += 10;

        // Clamps variables to sensible values
        speed = Mathf.Clamp(speed, 5f, 50f);
        sensX = Mathf.Clamp(sensX, 10f, 750f);
        sensY = Mathf.Clamp(sensY, 10f, 750f);

        // Update overlay text
        overlay.text = string.Format("Speed (1 & 2) = {0}, SensX (3 & 4) = {1}, SensY (5 & 6) = {2}",
            speed,
            sensX,
            sensY);
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
