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

    [Header("Основа Карты")]
    public Renderer groundRenderer;
    public Terrain groundTerrain;
    public float boundsPadding = 0f;

    [Header("Ограничения зума")]
    public float minZoomY = 5f;
    public float maxZoomY = 30f;

    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;

    private float currentPitch = 0f;
    private float currentYaw;

    void Start()
    {
        currentPitch = transform.eulerAngles.x;
        currentYaw = transform.eulerAngles.y;

        UpdateMapBounds();
        CalculateCameraLimits();
    }

    void CalculateCameraLimits()
    {
        if (groundTerrain != null)
        {
            Vector3 terrainPos = groundTerrain.transform.position;
            Vector3 terrainSize = groundTerrain.terrainData.size;

            minX = terrainPos.x;
            maxX = terrainPos.x + terrainSize.x;
            minZ = terrainPos.z;
            maxZ = terrainPos.z + terrainSize.z;
            
            Debug.Log($"Камера настроена под Terrain. Границы: X({minX} до {maxX}), Z({minZ} до {maxZ})");
        }
        else if (groundRenderer != null)
        {
            minX = groundRenderer.bounds.min.x;
            maxX = groundRenderer.bounds.max.x;
            minZ = groundRenderer.bounds.min.z;
            maxZ = groundRenderer.bounds.max.z;
            
            Debug.Log($"Камера настроена под Plane/Renderer. Границы: X({minX} до {maxX}), Z({minZ} до {maxZ})");
        }
        else
        {
            Debug.LogWarning("В CameraController не назначен пол! Камера может улететь за карту.");
            minX = -1000f; maxX = 1000f; minZ = -1000f; maxZ = 1000f;
        }
    }

    public void UpdateMapBounds()
    {
        if (groundRenderer != null)
        {
            minX = groundRenderer.bounds.min.x - boundsPadding;
            maxX = groundRenderer.bounds.max.x + boundsPadding;
            minZ = groundRenderer.bounds.min.z - boundsPadding;
            maxZ = groundRenderer.bounds.max.z + boundsPadding;
            
            Debug.Log($"<color=cyan>[Камера] Границы карты обновлены: X({minX} до {maxX}), Z({minZ} до {maxZ})</color>");
        }
        else
        {
            Debug.LogWarning("Plane (groundRenderer) не назначен в скрипт камеры!");
        }
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
        if (scroll != 0)
        {
            Vector3 nextPos = transform.position + transform.forward * scroll * zoomSpeed;

            if (nextPos.y >= minZoomY && nextPos.y <= maxZoomY)
            {
                transform.position = nextPos;
            }
        }

        if (groundRenderer != null)
        {
            Vector3 clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
            clampedPos.z = Mathf.Clamp(clampedPos.z, minZ, maxZ);
            clampedPos.y = Mathf.Clamp(clampedPos.y, minZoomY, maxZoomY);
            transform.position = clampedPos;
        }
        else
        {
            Vector3 clampedPos = transform.position;
            clampedPos.y = Mathf.Clamp(clampedPos.y, minZoomY, maxZoomY);
            transform.position = clampedPos;
        }
    }
}