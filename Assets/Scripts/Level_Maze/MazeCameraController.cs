using UnityEngine;

public class MazeCameraController : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0f, 15f, -7f);
    public float smoothSpeed = 5f;
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 120f;
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode rotateRightKey = KeyCode.E;

    private float currentYaw = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        if (Input.GetKey(rotateLeftKey))
            currentYaw += rotationSpeed * Time.deltaTime;
        if (Input.GetKey(rotateRightKey))
            currentYaw -= rotationSpeed * Time.deltaTime;

        Quaternion rotation = Quaternion.Euler(0, currentYaw, 0);
        Vector3 rotatedOffset = rotation * offset;

        Vector3 desiredPosition = target.position + rotatedOffset;
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(target);
    }
}