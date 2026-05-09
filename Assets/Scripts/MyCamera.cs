using UnityEngine;

public class MyCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 80f;
    public float fastMoveSpeedMultiplier = 2f;
    public float zoomSpeed = 40f;
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 60f;
    public float pitchSpeed = 40f;
    public float minPitchAngle = -60f;
    public float maxPitchAngle = 60f;
    
    [Header("Key Bindings")]
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode rotateRightKey = KeyCode.E;
    public KeyCode pitchUpKey = KeyCode.R;
    public KeyCode pitchDownKey = KeyCode.F;
    public KeyCode fastMoveKey = KeyCode.LeftShift;

    private float currentPitch = 0f;

    void Start()
    {
        currentPitch = transform.eulerAngles.x;
    }

    void Update()
    {
        float currentSpeed = moveSpeed;
        if (Input.GetKey(fastMoveKey))
        {
            currentSpeed *= fastMoveSpeedMultiplier;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        Vector3 moveDirection = transform.forward * v + transform.right * h;
        moveDirection.y = 0; 
        
        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        if (Input.GetKey(rotateLeftKey))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(rotateRightKey))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKey(pitchUpKey))
        {
            currentPitch -= pitchSpeed * Time.deltaTime;
        }
        if (Input.GetKey(pitchDownKey))
        {
            currentPitch += pitchSpeed * Time.deltaTime;
        }

        currentPitch = Mathf.Clamp(currentPitch, minPitchAngle, maxPitchAngle);
        transform.eulerAngles = new Vector3(currentPitch, transform.eulerAngles.y, 0f);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * zoomSpeed;
    }
}
