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

    [Header("Перемещение")]
    public float moveSpeed = 5f;
    
    [Header("ХП и Диапазон урона")]
    public float maxHealth = 100f;
    public float minDamage = 8f;
    public float maxDamage = 12f;
    
    [Tooltip("Шанс крита в процентах (0-100)")]
    public float critChance = 15f; 
    public float critMultiplier = 2.0f;

    [Tooltip("Игнорирование брони цели")]
    public float armorPenetration = 0f;

    [Tooltip("Параметры атаки")]
    public float attackSpeed = 1f;
    public float attackRange = 10f;

    [Header("Защита")]
    public float armor = 2f;

    [Tooltip("Шанс уворота от атаки в процентах (0-100)")]
    public float dodgeChance = 10f;

    [Header("Регенерация")]
    public bool canRegen = true;
    public float regenTickRate = 2f;
    [Tooltip("Процент от Max HP, который восстанавливается за тик")]
    public float regenPercentPerTick = 1f;

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

    public void AddXP(float amount)
    {
        currentXP += amount;
        float xpNeeded = GetXPForNextLevel();
        
        while (currentXP >= xpNeeded)
        {
            currentXP -= xpNeeded;
            LevelUp();
            xpNeeded = GetXPForNextLevel();
        }
    }

    public float GetXPForNextLevel()
    {
        return 100f * Mathf.Pow(1.5f, level - 1);
    }

    void LevelUp()
    {
        level++;
        
        maxHealth += 10f;
        minDamage += 1f;
        maxDamage += 2f;

        Health h = GetComponent<Health>();
        if (h != null)
        {
            h.maxHealth = maxHealth;
            h.Heal(10f);
            if (h.healthBar != null) h.healthBar.UpdateLevelText(level); 
        }

        if (level % 3 == 0)
        {
            armor += 1f;
            armorPenetration += 1f;
            Debug.Log($"<color=gold>{unitName} получил +1 к Броне и Пробитию за {level} уровень!</color>");
        }
    }
}