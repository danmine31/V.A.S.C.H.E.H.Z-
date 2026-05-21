using UnityEngine;

public enum Fraction { None, People, Mages, Robots }

public class UnitStats : MonoBehaviour
{
    [Header("Паспорт (Идентификация)")]
    public string unitName = "Unknown Unit";
    public Fraction unitFraction = Fraction.People;
    public int ownerID = 1;
    public int teamID = 1;

    [Header("Внешний вид")]
    [Tooltip("Цвет автоматически перезапишется из GameManager")]
    public Color unitColor = Color.white;

    [Header("Круг выделения")]
    public float selectionCircleRadius = 1.5f;
    private GameObject selectionCircleObj;

    [Header("Боевые характеристики")]
    public float maxHealth = 100f;
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float armor = 0f;

    [Header("Прокачка")]
    public int level = 1;
    public float currentXP = 0f;

    void Awake()
    {
        Health healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.maxHealth = this.maxHealth;
            if (healthComponent.currentHealth == 0)
            {
                healthComponent.currentHealth = this.maxHealth;
            }
        }
    }

    void Start()
    {
        UpdateDataFromManager();
        ApplyColorOptimized();
        CreateSelectionCircle();
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

    void UpdateDataFromManager()
    {
        GameManager gm = GameManager.Instance;
        
        if (gm == null) 
        {
            gm = Object.FindAnyObjectByType<GameManager>();
        }

        if (gm != null)
        {
            unitColor = gm.GetPlayerColor(ownerID);
            teamID = gm.GetPlayerTeam(ownerID);
        }
    }

    public void ApplyColorOptimized()
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

    void CreateSelectionCircle()
    {
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
}