using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Иконки курсора")]
    public Texture2D defaultCursor;   
    public Texture2D attackCursor;    
    public Texture2D gatherCursor;    
    public Texture2D lootCursor;
    public Texture2D arrowCursor;

    [Header("Слои для проверки")]
    public LayerMask interactableLayers; 

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        UpdateCursorIcon();
    }

    void UpdateCursorIcon()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool hasSelectedUnits = SelectionController.Instance != null && SelectionController.Instance.HasSelectedUnits();

        if (Physics.Raycast(ray, out hit, 1000f, interactableLayers))
        {
            Health targetHealth = hit.collider.GetComponentInParent<Health>();
            UnitStats stats = hit.collider.GetComponentInParent<UnitStats>();

            if (targetHealth != null && stats != null)
            {
                if (hasSelectedUnits)
                {
                    if (stats.ownerID == 1)
                    {
                        SetCursor(arrowCursor != null ? arrowCursor : defaultCursor, true);
                        return;
                    }
                    else if (stats.teamID != 1)
                    {
                        SetCursor(attackCursor, true);
                        return;
                    }
                    else
                    {
                        SetCursor(arrowCursor != null ? arrowCursor : defaultCursor, true);
                        return;
                    }
                }
                else
                {
                    SetCursor(arrowCursor != null ? arrowCursor : defaultCursor, true);
                    return;
                }
            }

            if (hasSelectedUnits && hit.collider.GetComponentInParent<ResourceSource>() != null)
            {
                SetCursor(gatherCursor, true);
                return;
            }

            if (hasSelectedUnits && hit.collider.GetComponentInParent<LootBox>() != null)
            {
                SetCursor(lootCursor, true);
                return;
            }
        }

        SetCursor(defaultCursor, false);
    }

    void SetCursor(Texture2D cursorTexture, bool centerHotspot)
    {
        if (cursorTexture == null) return;

        Vector2 hotspot = centerHotspot ? new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f) : Vector2.zero;
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.ForceSoftware);
    }
}