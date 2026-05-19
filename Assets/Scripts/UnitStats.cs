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
}