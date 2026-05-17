using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler
{
    [Header("Что двигаем?")]
    public RectTransform windowToDrag;

    private Canvas canvas;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (windowToDrag != null && canvas != null)
        {
            windowToDrag.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
}