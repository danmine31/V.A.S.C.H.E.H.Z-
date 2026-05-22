using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapController : MonoBehaviour, IPointerClickHandler
{
    [Header("Объекты")]
    public Transform mainCamera;
    public Renderer groundRenderer;

    [Header("Настройки камеры")]
    public Vector3 cameraOffset = new Vector3(0f, 0f, -20f); 
    public float edgePadding = 30f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (groundRenderer == null || mainCamera == null) return;

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
        
        Rect rect = rectTransform.rect;
        float percentX = (localPoint.x - rect.xMin) / rect.width;
        float percentY = (localPoint.y - rect.yMin) / rect.height;

        Bounds bounds = groundRenderer.bounds;

        float lookAtX = bounds.min.x + (percentX * bounds.size.x);
        float lookAtZ = bounds.min.z + (percentY * bounds.size.z);

        float clampedX = Mathf.Clamp(lookAtX, bounds.min.x + edgePadding, bounds.max.x - edgePadding);
        float clampedZ = Mathf.Clamp(lookAtZ, bounds.min.z + edgePadding, bounds.max.z - edgePadding);

        mainCamera.position = new Vector3(clampedX, mainCamera.position.y, clampedZ) + cameraOffset;
    }
}