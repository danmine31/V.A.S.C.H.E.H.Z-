using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct DropItem
{
    public ItemType itemType;
    public int minAmount;
    public int maxAmount;
    [Range(0f, 100f)] public float dropChance;
}

public class Health : MonoBehaviour
{
    private UnitStats stats;

    [Header("Прочность")]
    public float maxHealth = 100f; 
    public float currentHealth;

    [Header("Лут при смерти/разрушении")]
    public GameObject lootBoxPrefab;
    public List<DropItem> dropTable = new List<DropItem>();

    private Dictionary<UnitStats, float> damageContributors = new Dictionary<UnitStats, float>();

    [Header("UI")]
    public GameObject healthBarPrefab;
    [HideInInspector] public HealthBar healthBar;

    private float regenTimer = 0f;

    void Start()
    {
        stats = GetComponent<UnitStats>();

        if (stats != null) maxHealth = stats.maxHealth;
        if (currentHealth == 0) currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            GameObject hbObj = Instantiate(healthBarPrefab, this.transform);
            healthBar = hbObj.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.target = this.transform;
                healthBar.UpdateHealthBar(currentHealth, maxHealth);
                int lvl = stats != null ? stats.level : 1;
                healthBar.UpdateLevelText(lvl);
            }
        }
    }

    void Update()
    {
        if (stats != null && stats.canRegen && currentHealth < maxHealth && currentHealth > 0)
        {
            regenTimer += Time.deltaTime;
            
            if (regenTimer >= stats.regenTickRate)
            {
                float healAmount = maxHealth * (stats.regenPercentPerTick / 100f);
                Heal(healAmount);
                regenTimer = 0f;
            }
        }
    }

    public void Heal(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (healthBar != null) 
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
            int lvl = stats != null ? stats.level : 1;
            healthBar.UpdateLevelText(lvl);
        }
    }

    public void TakeDamage(float amount, UnitStats attacker, float armorPenetration = 0f)
    {
        if (stats != null)
        {
            if (Random.Range(0f, 100f) <= stats.dodgeChance)
            {
                Debug.Log($"<color=cyan>{stats.unitName} уклонился от атаки!</color>");
                return; 
            }
        }

        float finalDamage = amount;

        if (stats != null && attacker != null)
        {
            Fraction attackerFraction = attacker.unitFraction;

            if (attackerFraction == Fraction.Mages && stats.unitFraction == Fraction.People)
                finalDamage *= 1.5f;
            if (attackerFraction == Fraction.People && stats.unitFraction == Fraction.Robots)
                finalDamage *= 1.5f;
            if (attackerFraction == Fraction.Robots && stats.unitFraction == Fraction.Mages)
                finalDamage *= 1.5f;

            float effectiveArmor = Mathf.Max(0, stats.armor - armorPenetration);
            finalDamage -= effectiveArmor;
            
            if (finalDamage < 1f) finalDamage = 1f;
        }

        if (attacker != null && attacker.teamID != 0)
        {
            if (!damageContributors.ContainsKey(attacker)) damageContributors[attacker] = 0f;
            damageContributors[attacker] += finalDamage;
        }

        currentHealth -= finalDamage;

        if (healthBar != null) 
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
            int lvl = stats != null ? stats.level : 1;
            healthBar.UpdateLevelText(lvl);
        }
        
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        DistributeXP();
        SpawnLoot();
        
        if (healthBar != null) Destroy(healthBar.gameObject);
        Destroy(gameObject);
        UnitController controller = GetComponent<UnitController>();
        if (controller != null && controller.teamID == 1)
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.UnregisterPlayerUnit();
            }
        }
    }

    void DistributeXP()
    {
        float totalDamage = 0f;
        foreach (var dmg in damageContributors.Values) totalDamage += dmg;
        if (totalDamage <= 0) return;

        float xpReward = maxHealth;

        foreach (var kvp in damageContributors)
        {
            if (kvp.Key != null && kvp.Key.gameObject != null)
            {
                float share = kvp.Value / totalDamage;
                kvp.Key.AddXP(xpReward * share);
            }
        }
    }

    void SpawnLoot()
    {
        if (lootBoxPrefab == null || dropTable.Count == 0) return;

        List<LootBox.LootItem> itemsToDrop = new List<LootBox.LootItem>();

        foreach (var drop in dropTable)
        {
            if (Random.Range(0f, 100f) <= drop.dropChance)
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                if (amount > 0)
                {
                    itemsToDrop.Add(new LootBox.LootItem { itemType = drop.itemType, amount = amount });
                }
            }
        }

        if (itemsToDrop.Count > 0)
        {
            GameObject boxObj = Instantiate(lootBoxPrefab, transform.position, Quaternion.identity);
            LootBox box = boxObj.GetComponent<LootBox>();
            if (box != null)
            {
                box.boxContents.Clear();
                foreach (var item in itemsToDrop)
                {
                    box.AddItem(item.itemType, item.amount);
                }
            }
        }
    }
}