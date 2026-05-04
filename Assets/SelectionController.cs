using UnityEngine;
using System.Collections.Generic;

public class SelectionController : MonoBehaviour
{
    public RectTransform selectionBoxVisual;
    public LayerMask groundLayer;

    private List<UnitMover> selectedUnits = new List<UnitMover>();
    private Vector2 startMousePos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Input.mousePosition;
            if (selectionBoxVisual != null) selectionBoxVisual.gameObject.SetActive(true);
        }
        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox();
        }
        if (Input.GetMouseButtonUp(0))
        {
            SelectUnits();
            if (selectionBoxVisual != null) selectionBoxVisual.gameObject.SetActive(false);
        }
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            MoveSelectedUnits();
        }
    }

    void UpdateSelectionBox()
    {
        if (selectionBoxVisual == null) return;

        Vector2 currentMousePos = Input.mousePosition;
        float width = currentMousePos.x - startMousePos.x;
        float height = currentMousePos.y - startMousePos.y;

        selectionBoxVisual.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBoxVisual.anchoredPosition = startMousePos + new Vector2(width / 2, height / 2);
    }

    void SelectUnits()
    {
        foreach (var unit in selectedUnits) unit.SetSelected(false);
        selectedUnits.Clear();

        Rect selectionRect = new Rect(
            Mathf.Min(startMousePos.x, Input.mousePosition.x),
            Mathf.Min(startMousePos.y, Input.mousePosition.y),
            Mathf.Abs(startMousePos.x - Input.mousePosition.x),
            Mathf.Abs(startMousePos.y - Input.mousePosition.y)
        );

        UnitMover[] allUnits = FindObjectsOfType<UnitMover>();
        foreach (UnitMover unit in allUnits)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
            if (selectionRect.Contains(screenPos))
            {
                selectedUnits.Add(unit);
                unit.SetSelected(true);
            }
        }
    }

    void MoveSelectedUnits()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, groundLayer))
        {
            foreach (var unit in selectedUnits)
            {
                unit.MoveTo(hit.point);
            }
        }
    }
}


