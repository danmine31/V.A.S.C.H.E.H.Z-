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

    private UnitStats inspectedUnit;

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
        selectedUnits.RemoveAll(unit => unit == null);
        
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

    void ClearInspected()
    {
        if (inspectedUnit != null)
        {
            inspectedUnit.SetSelected(false);
            inspectedUnit = null;
        }
    }

    void SelectSingleUnit()
    {
        selectedUnits.RemoveAll(u => u == null);
        foreach (var unit in selectedUnits) unit.GetComponent<UnitStats>().SetSelected(false);
        selectedUnits.Clear();

        if (inspectedUnit != null)
        {
            inspectedUnit.SetSelected(false);
            inspectedUnit = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, unitLayer))
        {
            UnitStats stats = hit.collider.GetComponentInParent<UnitStats>();

            if (stats != null)
            {
                if (stats.ownerID == 1)
                {
                    UnitController controller = stats.GetComponent<UnitController>();
                    if (controller != null)
                    {
                        selectedUnits.Add(controller);
                        stats.SetSelected(true);
                    }
                }
                else
                {
                    inspectedUnit = stats;
                    stats.SetSelected(true);
                    Debug.Log($"<color=yellow>Осмотр: {stats.unitName}</color>");
                }
            }
        }
    }

    void SelectUnitsInBox()
    {
        selectedUnits.RemoveAll(u => u == null);
        foreach (var unit in selectedUnits) unit.GetComponent<UnitStats>().SetSelected(false);
        selectedUnits.Clear();
        
        if (inspectedUnit != null)
        {
            inspectedUnit.SetSelected(false);
            inspectedUnit = null;
        }

        Rect selectionRect = new Rect(
            Mathf.Min(startMousePos.x, Input.mousePosition.x),
            Mathf.Min(startMousePos.y, Input.mousePosition.y),
            Mathf.Abs(startMousePos.x - Input.mousePosition.x),
            Mathf.Abs(startMousePos.y - Input.mousePosition.y)
        );

        var allStats = Object.FindObjectsByType<UnitStats>(FindObjectsInactive.Exclude);
        
        foreach (UnitStats stats in allStats)
        {
            if (stats.ownerID == 1)
            {
                UnitController controller = stats.GetComponent<UnitController>();
                if (controller != null)
                {
                    Vector2 screenPos = Camera.main.WorldToScreenPoint(stats.transform.position);
                    if (selectionRect.Contains(screenPos))
                    {
                        selectedUnits.Add(controller);
                        stats.SetSelected(true);
                    }
                }
            }
        }

        if (selectedUnits.Count == 0)
        {
            foreach (UnitStats stats in allStats)
            {
                if (stats.ownerID != 1)
                {
                    Vector2 screenPos = Camera.main.WorldToScreenPoint(stats.transform.position);
                    if (selectionRect.Contains(screenPos))
                    {
                        inspectedUnit = stats;
                        stats.SetSelected(true);
                        break;
                    }
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
            UnitController enemyController = hit.collider.GetComponentInParent<UnitController>();

            if (enemyHealth != null && enemyStats != null)
            {
                if (enemyStats.teamID != 1 && enemyStats.teamID != 0)
                {
                    foreach (var unit in selectedUnits) unit.SetTarget(enemyHealth);
                    return;
                }
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
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                if (i == 0) 
                {
                    selectedUnits[i].MoveTo(hit.point); 
                }
                else 
                {
                    float angle = i * 137.5f * Mathf.Deg2Rad; 
                    float radius = 1.5f * Mathf.Sqrt(i); 
                    
                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                    selectedUnits[i].MoveTo(hit.point + offset);
                }
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

    public List<UnitController> GetSelectedUnits()
    {
        return selectedUnits;
    }

    public UnitController GetMainSelectedUnit()
    {
        if (selectedUnits.Count > 0)
        {
            return selectedUnits[0];
        }
        return null;
    }

    public UnitStats GetInspectedUnit()
    {
        return inspectedUnit;
    }
}