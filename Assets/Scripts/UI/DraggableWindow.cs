using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler
{
    [Header("Что двигаем?")]
    public RectTransform windowToDrag;

    private Canvas canvas;
    private RectTransform canvasRect;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (windowToDrag != null && canvas != null)
        {
            windowToDrag.anchoredPosition += eventData.delta / canvas.scaleFactor;

            Vector3[] canvasCorners = new Vector3[4];
            canvasRect.GetWorldCorners(canvasCorners);

            Vector3[] windowCorners = new Vector3[4];
            windowToDrag.GetWorldCorners(windowCorners);

            Vector3 offset = Vector3.zero;

            if (windowCorners[0].x < canvasCorners[0].x) 
                offset.x = canvasCorners[0].x - windowCorners[0].x;
            
            if (windowCorners[2].x > canvasCorners[2].x) 
                offset.x = canvasCorners[2].x - windowCorners[2].x;
            
            if (windowCorners[0].y < canvasCorners[0].y) 
                offset.y = canvasCorners[0].y - windowCorners[0].y;
            
            if (windowCorners[2].y > canvasCorners[2].y) 
                offset.y = canvasCorners[2].y - windowCorners[2].y;

            windowToDrag.position += offset;
        }
    }
}