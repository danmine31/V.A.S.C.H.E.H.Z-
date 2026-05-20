using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Иконки курсора")]
    public Texture2D defaultCursor;   
    public Texture2D attackCursor;    
    public Texture2D gatherCursor;    
    public Texture2D lootCursor;      

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
        if (SelectionController.Instance == null || !SelectionController.Instance.HasSelectedUnits())
        {
            SetCursor(defaultCursor, false);
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, interactableLayers))
        {
            Health targetHealth = hit.collider.GetComponentInParent<Health>();
            if (targetHealth != null)
            {
                UnitStats stats = hit.collider.GetComponentInParent<UnitStats>();
                if (stats != null)
                {
                    if (stats.ownerID != 1 && stats.ownerID != 0)
                    {
                        SetCursor(attackCursor, true);
                        return;
                    }
                }
                else
                {
                    SetCursor(attackCursor, true);
                    return;
                }
            }

            if (hit.collider.GetComponentInParent<ResourceSource>() != null)
            {
                SetCursor(gatherCursor, true);
                return;
            }

            if (hit.collider.GetComponentInParent<LootBox>() != null)
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