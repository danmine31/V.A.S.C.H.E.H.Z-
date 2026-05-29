using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    [Header("Звуки")]
    public AudioClip deathSound;

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
    public GameObject floatingTextPrefab;

    private float regenTimer = 0f;
    private bool isHealing = false;

    void Start()
    {      оо
        stats = GetComponent<UnitStats>();

        if (stats != null) maxHealth = stats.maxHealth;
        if (currentHealth == 0) currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            GameObject hbObj = Instantiate(healthBarPrefab, this.transform);
            healthBar = hbObj.GetComponent<HealthBar>();
            Debug.Log($"[{gameObject.name}] Слайдер MaxValue: {healthBar.slider.maxValue}, Value: {healthBar.slider.value}");

            if (healthBar != null)
            {
                healthBar.target = this.transform;
                healthBar.UpdateHealthBar(currentHealth, maxHealth);
                int lvl = stats != null ? stats.level : 1;
                healthBar.UpdateLevelText(lvl);

                if (GameManager.Instance != null && stats != null)
                {
                    Color realColor = GameManager.Instance.GetPlayerColor(stats.ownerID);
                    healthBar.SetColor(realColor);
                }
            }
        }
        Debug.Log($"[{gameObject.name}] Мой OwnerID: {stats.ownerID}. Цвет из менеджера: {GameManager.Instance.GetPlayerColor(stats.ownerID)}");
        Debug.Log($"[{gameObject.name}] HP: {currentHealth} / {maxHealth}");
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

    public void TryStartHealing()
    {
        if (isHealing || currentHealth >= maxHealth) return;
        
        UnitInventory inventory = GetComponent<UnitInventory>();
        if (inventory != null && inventory.GetItemCount(ItemType.Medkit) > 0)
        {
            UnitAI ai = GetComponent<UnitAI>();
            if (ai != null) ai.isManualControl = true; 

            UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }

            StartCoroutine(HealingProcessCoroutine(inventory, ai));
        }
    }

    private IEnumerator HealingProcessCoroutine(UnitInventory inventory, UnitAI ai)
    {
        isHealing = true;
        Debug.Log($"<color=green>{stats.unitName} лечится (5 секунд)...</color>");
        
        float timer = 0f;
        Vector3 startPos = transform.position;

        while (timer < 5f)
        {
            timer += Time.deltaTime;
            
            if (healthBar != null) healthBar.UpdateActionBar(timer / 5f);

            if (Vector3.Distance(startPos, transform.position) > 0.5f)
            {
                Debug.Log($"<color=red>Лечение прервано!</color>");
                isHealing = false;
                if (ai != null) ai.isManualControl = false;
                if (healthBar != null) healthBar.UpdateActionBar(0f);
                yield break; 
            }
            yield return null; 
        }

        if (healthBar != null) healthBar.UpdateActionBar(0f);

        if (inventory != null && inventory.RemoveItem(ItemType.Medkit, 1))
        {
            float healAmount = maxHealth * 0.5f;
            Heal(healAmount);
        }

        isHealing = false;
        if (ai != null) ai.isManualControl = false;
    }

    public void TakeDamage(float amount, UnitStats attacker, float armorPenetration = 0f, bool isCrit = false)
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

        if (floatingTextPrefab != null)
        {
            GameObject textObj = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            textObj.GetComponent<FloatingText>().Setup(finalDamage, isCrit, false); 
        }
        
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        DistributeXP();
        SpawnLoot();
        
        bool playerHelpedKill = false;
        foreach (var attacker in damageContributors.Keys)
        {
            if (attacker != null && attacker.ownerID == 1) { playerHelpedKill = true; break; }
        }

        if (playerHelpedKill && stats != null && stats.ownerID != 1 && GameManager.Instance != null)
        {
            GameManager.Instance.AddInfluence(4);
        }
        
        if (healthBar != null) Destroy(healthBar.gameObject);

        if (deathSound != null) 
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, 1.0f);
        }

        Destroy(gameObject);
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
        if (lootBoxPrefab == null) return;

        List<LootBox.LootItem> itemsToDrop = new List<LootBox.LootItem>();

        if (stats != null && stats.ownerID == 1)
        {
            UnitInventory inv = GetComponent<UnitInventory>();
            if (inv != null)
            {
                foreach (var slot in inv.slots)
                {
                    itemsToDrop.Add(new LootBox.LootItem { itemType = slot.itemType, amount = slot.amount });
                }
            }
        }
        else
        {
            if (dropTable.Count > 0)
            {
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