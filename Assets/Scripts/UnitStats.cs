using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [Header("Паспорт (Идентификация)")]
    public string unitName = "Unknown Unit";
    public int ownerID = 1;
    public int teamID = 1;

    [Header("Внешний вид")]
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
        SyncHealth();
    }

    void Start()
    {
        ApplyColorOptimized();
    }

    void ApplyColorOptimized()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        foreach (Renderer rend in renderers)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            rend.GetPropertyBlock(propBlock);
            
            propBlock.SetColor("_BaseColor", unitColor); 
            
            rend.SetPropertyBlock(propBlock);
        }
    }

    void SyncHealth()
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
}