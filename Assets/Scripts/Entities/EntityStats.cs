using UnityEngine;

public enum Fraction { None, People, Mages, Robots }

public class EntityStats : MonoBehaviour
{
    [Header("Базовый Паспорт (Entity)")]
    public string entityName = "Unknown Entity";
    public Fraction entityFraction = Fraction.None;
    public int ownerID = 0;
    public int teamID = 0;

    [Header("Внешний вид")]
    [Tooltip("Цвет автоматически перезапишется из GameManager")]
    public Color unitColor = Color.white;

    [Header("Круг выделения (Для Entity)")]
    public bool hasSelectionCircle = true;
    public float selectionCircleRadius = 1.5f;
    protected GameObject selectionCircleObj;

    [Header("Базовые характеристики")]
    public float maxHealth = 100f;
    public float armor = 2f;

    protected virtual void Start()
    {
        UpdateDataFromManager();
        ApplyColorOptimized();
        CreateSelectionCircle();
    }

    public void UpdateDataFromManager()
    {
        if (GameManager.Instance != null)
        {
            unitColor = GameManager.Instance.GetPlayerColor(ownerID);
            teamID = GameManager.Instance.GetPlayerTeam(ownerID);
        }
    }

    public virtual void ApplyColorOptimized()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            if (rend == null) continue;
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            rend.GetPropertyBlock(propBlock);
            propBlock.SetColor("_BaseColor", unitColor); 
            rend.SetPropertyBlock(propBlock);
        }
    }

    protected virtual void CreateSelectionCircle()
    {
        if (!hasSelectionCircle) return;
        
        if (selectionCircleObj != null) Destroy(selectionCircleObj);
        selectionCircleObj = new GameObject("SelectionCircle");
        selectionCircleObj.transform.SetParent(transform);
        
        Collider col = GetComponentInChildren<Collider>();
        if (col == null) return;

        float bottomY = col.bounds.min.y;
        selectionCircleObj.transform.position = new Vector3(transform.position.x, bottomY + 0.05f, transform.position.z);
        selectionCircleObj.transform.localRotation = Quaternion.identity;

        Vector3 lossy = transform.lossyScale;
        selectionCircleObj.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);

        LineRenderer lr = selectionCircleObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = 0.2f; 
        lr.endWidth = 0.2f;

        Color finalColor = unitColor;
        finalColor.a = 0.9f;
        lr.startColor = finalColor;
        lr.endColor = finalColor;

        int segments = 40;
        lr.positionCount = segments + 1;
        float maxExtents = Mathf.Max(col.bounds.extents.x, col.bounds.extents.z);
        float radius = maxExtents * 1.5f; 

        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            lr.SetPosition(i, new Vector3(x, 0f, z));
            angle += 360f / segments;
        }

        selectionCircleObj.SetActive(false);
    }
    
    public void SetSelected(bool isSelected)
    {
        if (selectionCircleObj != null) selectionCircleObj.SetActive(isSelected);
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            UpdateDataFromManager();
            ApplyColorOptimized();
        }
    }
    #endif
}