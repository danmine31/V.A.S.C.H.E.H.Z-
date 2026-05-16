using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 80f;
    public float fastMoveSpeedMultiplier = 2f;
    public float zoomSpeed = 40f;
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 60f;
    public float pitchSpeed = 40f;
    public float minPitchAngle = 10f;
    public float maxPitchAngle = 80f;
    
    [Header("Key Bindings")]
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode rotateRightKey = KeyCode.E;
    public KeyCode pitchUpKey = KeyCode.R;
    public KeyCode pitchDownKey = KeyCode.F;
    public KeyCode fastMoveKey = KeyCode.LeftShift;

    [Header("Настройки мыши")]
    public float mouseSensitivity = 5f;

    private float currentPitch = 0f;
    private float currentYaw;

    void Start()
    {
        currentPitch = transform.eulerAngles.x;
        currentYaw = transform.eulerAngles.y;
    }

    void Update()
    {
        float currentSpeed = moveSpeed;
        if (Input.GetKey(fastMoveKey)) currentSpeed *= fastMoveSpeedMultiplier;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * v + transform.right * h;
        moveDirection.y = 0;
        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        if (Input.GetKey(rotateLeftKey)) currentYaw -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(rotateRightKey)) currentYaw += rotationSpeed * Time.deltaTime;

        if (Input.GetKey(pitchUpKey)) currentPitch -= pitchSpeed * Time.deltaTime;
        if (Input.GetKey(pitchDownKey)) currentPitch += pitchSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            currentYaw += mouseX;
            currentPitch -= mouseY; 
        }

        currentPitch = Mathf.Clamp(currentPitch, minPitchAngle, maxPitchAngle);
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * zoomSpeed;
    }
}