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

        var allUnits = Object.FindObjectsByType<UnitMover>(FindObjectsInactive.Exclude);
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

    public LayerMask enemyLayer;

    void MoveSelectedUnits()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, enemyLayer))
        {
            Health enemyHealth = hit.collider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                foreach (var unit in selectedUnits)
                {
                    Debug.Log("Приказ: Атаковать " + hit.collider.name);
                    unit.SetTarget(enemyHealth);
                }
                return;
            }
        }

        if (Physics.Raycast(ray, out hit, 1000, groundLayer))
        {
            foreach (var unit in selectedUnits)
            {
                unit.MoveTo(hit.point);
            }
        }
    }
}


