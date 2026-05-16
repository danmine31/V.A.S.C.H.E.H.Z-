using UnityEngine;
using System.Collections.Generic;

public class SelectionController : MonoBehaviour
{
    public RectTransform selectionBoxVisual;
    public LayerMask groundLayer;
    public LayerMask unitLayer;
    public LayerMask resourceLayer; 

    private List<UnitController> selectedUnits = new List<UnitController>();
    private Vector2 startMousePos;
    private bool isBoxSelecting = false;
    public static bool isRadiusesVisible = false;

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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isRadiusesVisible = true;
            SetAllRadiusesVisible(true);
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            isRadiusesVisible = false;
            SetAllRadiusesVisible(false);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isBoxSelecting) SelectUnitsInBox();
            else SelectSingleUnit();
            
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
        selectedUnits.RemoveAll(u => u == null);
        foreach (var unit in selectedUnits) unit.SetSelected(false);
        selectedUnits.Clear();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, unitLayer))
        {
            UnitController unit = hit.collider.GetComponent<UnitController>();
            Health health = hit.collider.GetComponent<Health>();

            if (unit != null && health != null && health.teamID == 1)
            {
                selectedUnits.Add(unit);
                unit.SetSelected(true);
                Debug.Log("Выбран юнит: " + unit.name);
            }
        }
    }

    void SelectUnitsInBox()
    {
        selectedUnits.RemoveAll(u => u == null);
        foreach (var unit in selectedUnits) unit.SetSelected(false);
        selectedUnits.Clear();

        Rect selectionRect = new Rect(
            Mathf.Min(startMousePos.x, Input.mousePosition.x),
            Mathf.Min(startMousePos.y, Input.mousePosition.y),
            Mathf.Abs(startMousePos.x - Input.mousePosition.x),
            Mathf.Abs(startMousePos.y - Input.mousePosition.y)
        );

        var allUnits = Object.FindObjectsByType<UnitController>(FindObjectsInactive.Exclude);
        foreach (UnitController unit in allUnits)
        {
            Health health = unit.GetComponent<Health>();
            
            if (health != null && health.teamID == 1)
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

    void MoveOrAttack()
    {
        selectedUnits.RemoveAll(unit => unit == null);
        if (selectedUnits.Count == 0) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, unitLayer))
        {
            Health enemyHealth = hit.collider.GetComponent<Health>();
            if (enemyHealth != null)
            {
                if (enemyHealth.teamID != 1) 
                {
                    foreach (var unit in selectedUnits)
                    {
                        unit.SetTarget(enemyHealth);
                        Debug.Log("Приказ: атаковать " + hit.collider.name);
                    }
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        if (Physics.Raycast(ray, out hit, 1000, resourceLayer))
        {
            ResourceSource resource = hit.collider.GetComponent<ResourceSource>();
            if (resource != null)
            {
                foreach (var unit in selectedUnits)
                {
                    unit.SetResourceTarget(resource);
                    Debug.Log("Приказ: добывать ресурс " + hit.collider.name);
                }
                return;
            }
        }

        if (Physics.Raycast(ray, out hit, 1000, groundLayer))
        {
            foreach (var unit in selectedUnits)
            {
                unit.MoveTo(hit.point);
                Debug.Log("Приказ: идти в точку " + hit.point);
            }
        }
    }

    void SetAllRadiusesVisible(bool visible)
    {
        RadiusVisualizer[] visualizers = FindObjectsByType<RadiusVisualizer>(FindObjectsInactive.Exclude);
        foreach (var vis in visualizers)
        {
            vis.ToggleRadiuses(visible);
        }
    }
}

