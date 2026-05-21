using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MinimapCameraFrustum : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Camera mainCam;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

    [Header("Настройки линии")]
    public float lineWidth = 2f;
    public Color lineColor = Color.white;
    public float heightOffset = 50f;

    void Start()
    {
        mainCam = GetComponent<Camera>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 4;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = true;
        
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }

    void Update()
    {
        Vector3[] corners = new Vector3[4];
        
        corners[0] = GetGroundPosition(new Vector3(0, 0, 0));
        corners[1] = GetGroundPosition(new Vector3(0, 1, 0));
        corners[2] = GetGroundPosition(new Vector3(1, 1, 0));
        corners[3] = GetGroundPosition(new Vector3(1, 0, 0));

        lineRenderer.SetPositions(corners);
    }

    Vector3 GetGroundPosition(Vector3 viewportPos)
    {
        Ray ray = mainCam.ViewportPointToRay(viewportPos);
        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance) + Vector3.up * heightOffset; 
        }
        return Vector3.zero;
    }
}