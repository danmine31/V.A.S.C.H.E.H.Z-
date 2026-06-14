using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SelectionController : MonoBehaviour
{
    public RectTransform selectionBoxVisual;
    public LayerMask groundLayer;
    public LayerMask unitLayer;
    public LayerMask buildingLayer;
    public LayerMask vehicleLayer;
    public LayerMask resourceLayer; 

    private List<EntityController> selectedControllers = new List<EntityController>();
    private EntityStats inspectedEntity;

    private Vector2 startMousePos;
    private bool isBoxSelecting = false;
    private bool startedClickOnUI = false;
    public static bool isRadiusesVisible = false;

    public static SelectionController Instance;

    void Awake()
    {
        SelectionController[] allControllers = FindObjectsByType<SelectionController>(FindObjectsInactive.Include);
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
                    else Destroy(ctrl);
                }
            }
        }
        Instance = this;
    }

    public bool HasSelectedUnits()
    {
        return selectedControllers.Count > 0;
    }

    void Update()
    {
        selectedControllers.RemoveAll(c => c == null);
        
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
            if (isBoxSelecting) UpdateSelectionBox();
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

            if (isBoxSelecting) SelectEntitiesInBox();
            else SelectSingleEntity();
            
            if (selectionBoxVisual != null) selectionBoxVisual.gameObject.SetActive(false);
            isBoxSelecting = false;
        }

        if (Input.GetMouseButtonDown(1) && selectedControllers.Count > 0)
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

    void SelectSingleEntity()
    {
        selectedControllers.RemoveAll(c => c == null);
        foreach (var ctrl in selectedControllers) ctrl.GetComponent<EntityStats>().SetSelected(false);
        selectedControllers.Clear();

        if (inspectedEntity != null)
        {
            inspectedEntity.SetSelected(false);
            inspectedEntity = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int entityMask = unitLayer | buildingLayer | vehicleLayer;

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, entityMask))
        {
            EntityStats stats = hit.collider.GetComponentInParent<EntityStats>();
            if (stats != null)
            {
                if (stats.ownerID == 1)
                {
                    EntityController controller = stats.GetComponent<EntityController>();
                    if (controller != null)
                    {
                        selectedControllers.Add(controller);
                        stats.SetSelected(true);
                    }
                    else
                    {
                        inspectedEntity = stats;
                        stats.SetSelected(true);
                    }
                }
                else
                {
                    inspectedEntity = stats;
                    stats.SetSelected(true);
                }
            }
        }
    }

    void SelectEntitiesInBox()
    {
        selectedControllers.RemoveAll(c => c == null);
        foreach (var ctrl in selectedControllers) ctrl.GetComponent<EntityStats>().SetSelected(false);
        selectedControllers.Clear();
        
        if (inspectedEntity != null)
        {
            inspectedEntity.SetSelected(false);
            inspectedEntity = null;
        }

        Rect selectionRect = new Rect(
            Mathf.Min(startMousePos.x, Input.mousePosition.x),
            Mathf.Min(startMousePos.y, Input.mousePosition.y),
            Mathf.Abs(startMousePos.x - Input.mousePosition.x),
            Mathf.Abs(startMousePos.y - Input.mousePosition.y)
        );

        var allStats = Object.FindObjectsByType<EntityStats>(FindObjectsInactive.Exclude);
        
        foreach (EntityStats stats in allStats)
        {
            if (stats.ownerID == 1)
            {
                EntityController controller = stats.GetComponent<EntityController>();
                if (controller != null)
                {
                    Vector2 screenPos = Camera.main.WorldToScreenPoint(stats.transform.position);
                    if (selectionRect.Contains(screenPos))
                    {
                        selectedControllers.Add(controller);
                        stats.SetSelected(true);
                    }
                }
            }
        }

        if (selectedControllers.Count == 0)
        {
            foreach (EntityStats stats in allStats)
            {
                if (stats.ownerID != 1)
                {
                    Vector2 screenPos = Camera.main.WorldToScreenPoint(stats.transform.position);
                    if (selectionRect.Contains(screenPos))
                    {
                        inspectedEntity = stats;
                        stats.SetSelected(true);
                        break; 
                    }
                }
            }
        }
    }

    void MoveOrAttack()
    {
        selectedControllers.RemoveAll(c => c == null);
        if (selectedControllers.Count == 0) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f)) 
        {
            LootBox box = hit.collider.GetComponentInParent<LootBox>();
            if (box != null)
            {
                foreach (var ctrl in selectedControllers) ctrl.MoveTo(hit.point);
                box.InteractWithBox(selectedControllers); 
                return;
            }
        }

        int entityMask = unitLayer | buildingLayer | vehicleLayer;
        if (Physics.Raycast(ray, out hit, 1000f, entityMask))
        {
            Health enemyHealth = hit.collider.GetComponentInParent<Health>();
            EntityStats enemyStats = hit.collider.GetComponentInParent<EntityStats>();

            if (enemyHealth != null && enemyStats != null)
            {
                if (enemyStats.teamID != 1 && enemyStats.teamID != 0)
                {
                    foreach (var ctrl in selectedControllers) ctrl.SetTarget(enemyHealth);
                    return;
                }
            }
        }

        if (Physics.Raycast(ray, out hit, 1000f, resourceLayer))
        {
            ResourceSource resource = hit.collider.GetComponentInParent<ResourceSource>();
            if (resource != null)
            {
                foreach (var ctrl in selectedControllers) ctrl.SetResourceTarget(resource);
                return;
            }
        }

        if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
        {
            for (int i = 0; i < selectedControllers.Count; i++)
            {
                if (i == 0) 
                {
                    selectedControllers[i].MoveTo(hit.point); 
                }
                else 
                {
                    float angle = i * 137.5f * Mathf.Deg2Rad; 
                    float radius = 1.5f * Mathf.Sqrt(i); 
                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                    selectedControllers[i].MoveTo(hit.point + offset);
                }
            }
        }
    }

    void SetAllRadiusesVisible(bool visible)
    {
        RadiusVisualizer[] visualizers = FindObjectsByType<RadiusVisualizer>(FindObjectsInactive.Exclude);
        foreach (var vis in visualizers) vis.ToggleRadiuses(visible);
    }

    public List<EntityController> GetSelectedControllers()
    {
        return selectedControllers;
    }

    public EntityController GetMainSelectedController()
    {
        if (selectedControllers.Count > 0) return selectedControllers[0];
        return null;
    }

    public EntityStats GetInspectedEntity()
    {
        return inspectedEntity;
    }
}