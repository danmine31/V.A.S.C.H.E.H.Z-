using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SelectionController : MonoBehaviour
{
    public RectTransform selectionBoxVisual;
    public LayerMask groundLayer;
    public LayerMask unitLayer;
    public LayerMask resourceLayer; 

    private List<UnitController> selectedUnits = new List<UnitController>();
    private Vector2 startMousePos;
    private bool isBoxSelecting = false;
    private bool startedClickOnUI = false;
    public static bool isRadiusesVisible = false;

    public static SelectionController Instance;

    void Awake()
    {
        SelectionController[] allControllers = FindObjectsByType<SelectionController>(FindObjectsSortMode.None);
        
        if (allControllers.Length > 1)
        {
            foreach (var ctrl in allControllers)
            {
                if (ctrl != this)
                {
                    if (this.selectionBoxVisual == null && ctrl.selectionBoxVisual != null)
                    {
                        Destroy(this);
                        return;
                    }
                    else
                    {
                        Destroy(ctrl);
                    }
                }
            }
        }
        
        Instance = this;
    }

    public bool HasSelectedUnits()
    {
        return selectedUnits.Count > 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) 
            {
                startedClickOnUI = true;
                return;
            }
            
            startedClickOnUI = false;
            startMousePos = Input.mousePosition;
            isBoxSelecting = false;
            if (selectionBoxVisual != null) selectionBoxVisual.gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(0))
        {
            if (startedClickOnUI) return;

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
            if (startedClickOnUI) 
            {
                startedClickOnUI = false;
                return;
            }

            if (isBoxSelecting) SelectUnitsInBox();
            else SelectSingleUnit();
            
            if (selectionBoxVisual != null) selectionBoxVisual.gameObject.SetActive(false);
            isBoxSelecting = false;
        }

        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            
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

        if (Physics.Raycast(ray, out hit, 1000f, unitLayer))
        {
            UnitController unit = hit.collider.GetComponentInParent<UnitController>();
            UnitStats stats = hit.collider.GetComponentInParent<UnitStats>();

            if (unit != null && stats != null && stats.ownerID == 1)
            {
                selectedUnits.Add(unit);
                unit.SetSelected(true);
                Debug.Log($"<color=cyan>Одиночный клик: {stats.unitName} добавлен в отряд!</color>");
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
            UnitStats stats = unit.GetComponent<UnitStats>();
            
            if (stats != null && stats.ownerID == 1)
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

        if (Physics.Raycast(ray, out hit, 1000f)) 
        {
            LootBox box = hit.collider.GetComponentInParent<LootBox>();
            if (box != null)
            {
                foreach (var unit in selectedUnits) unit.MoveTo(hit.point);
                box.InteractWithBox(selectedUnits); 
                return;
            }
        }

        if (Physics.Raycast(ray, out hit, 1000f, unitLayer))
        {
            Health enemyHealth = hit.collider.GetComponentInParent<Health>();
            UnitStats enemyStats = hit.collider.GetComponentInParent<UnitStats>();

            if (enemyHealth != null && enemyStats != null)
            {
                if (enemyStats.teamID != 1) 
                {
                    foreach (var unit in selectedUnits) unit.SetTarget(enemyHealth);
                    return;
                }
                else return;
            }
        }

        if (Physics.Raycast(ray, out hit, 1000f, resourceLayer))
        {
            ResourceSource resource = hit.collider.GetComponentInParent<ResourceSource>();
            if (resource != null)
            {
                foreach (var unit in selectedUnits) unit.SetResourceTarget(resource);
                return;
            }
        }

        if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
        {
            foreach (var unit in selectedUnits) unit.MoveTo(hit.point);
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

    public UnitController GetMainSelectedUnit()
    {
        if (selectedUnits.Count > 0)
        {
            return selectedUnits[0];
        }
        return null;
    }
}