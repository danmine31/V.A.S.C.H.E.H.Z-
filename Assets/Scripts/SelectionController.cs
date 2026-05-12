using UnityEngine;
using System.Collections.Generic;

public class SelectionController : MonoBehaviour
{
    public RectTransform selectionBoxVisual;
    public LayerMask groundLayer;
    public LayerMask unitLayer;
    public LayerMask enemyLayer;

    private List<UnitMover> selectedUnits = new List<UnitMover>();
    private Vector2 startMousePos;
    private bool isBoxSelecting = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Input.mousePosition;
            isBoxSelecting = false;
            if (selectionBoxVisual != null) selectionBoxVisual.gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePos = Input.mousePosition;
            float distance = Vector2.Distance(startMousePos, currentMousePos);
            if (distance > 10f && !isBoxSelecting)
            {
                isBoxSelecting = true;
                if (selectionBoxVisual != null) selectionBoxVisual.gameObject.SetActive(true);
            }
            if (isBoxSelecting)
            {
                UpdateSelectionBox();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isBoxSelecting)
            {
                SelectUnitsInBox();
            }
            else
            {
                SelectSingleUnit();
            }
            if (selectionBoxVisual != null) selectionBoxVisual.gameObject.SetActive(false);
            isBoxSelecting = false;
        }

        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            MoveOrAttack();
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

    void SelectSingleUnit()
    {
        foreach (var unit in selectedUnits) unit.SetSelected(false);
        selectedUnits.Clear();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, unitLayer))
        {
            UnitMover unit = hit.collider.GetComponent<UnitMover>();
            if (unit != null)
            {
                selectedUnits.Add(unit);
                unit.SetSelected(true);
                Debug.Log("Выбран юнит: " + unit.name);
            }
        }
    }

    void SelectUnitsInBox()
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
            if (IsInLayerMask(unit.gameObject.layer, unitLayer))
            {
                Vector2 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
                if (selectionRect.Contains(screenPos))
                {
                    selectedUnits.Add(unit);
                    unit.SetSelected(true);
                }
            }
        }
        Debug.Log("Выбрано юнитов: " + selectedUnits.Count);
    }

    bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask & (1 << layer)) != 0;
    }

    public LayerMask resourceLayer;

    void MoveOrAttack()
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
                    Debug.Log("Приказ: атаковать " + hit.collider.name);
                    unit.SetTarget(enemyHealth);
                }
                return;
            }
        }

        if (Physics.Raycast(ray, out hit, 1000, resourceLayer))
        {
            ResourceSource resource = hit.collider.GetComponent<ResourceSource>();
            if (resource != null)
            {
                foreach (var unit in selectedUnits)
                {
                    Debug.Log("Приказ: добывать ресурс " + hit.collider.name);
                    unit.SetResourceTarget(resource);
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


