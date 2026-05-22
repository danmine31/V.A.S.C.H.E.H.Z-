using UnityEngine;
using System.Collections.Generic;

public enum SpawnerMode { Endless, ByCount, ByTime }

public class UnitSpawner : MonoBehaviour
{
    [Header("Настройки шаблона")]
    public GameObject unitPrefab;
    public Transform container;

    [Header("Настройки ИИ и Команды")]
    public AIBehavior spawnBehavior = AIBehavior.Defend;
    
    [Tooltip("Оставь 0, чтобы юнит использовал TeamID/OwnerID из своего префаба. Если укажешь цифру, спавнер перезапишет её.")]
    public int overrideOwnerID = 0;
    public int overrideTeamID = 0;

    [Header("Раненые бойцы")]
    [Range(1f, 100f)]
    public float spawnHealthPercent = 100f;

    [Header("Режим работы Спавнера")]
    public float spawnCooldown = 15f;
    public SpawnerMode mode = SpawnerMode.ByCount;

    [Header("Лимиты (в зависимости от режима)")]
    public int totalSpawnLimit = 50;
    public float survivalTime = 120f;

    [Tooltip("-1 означает бесконечное число живых")]
    public int maxAliveUnits = 10;

    [Header("Награда за зачистку/выживание")]
    public GameObject rewardLootBoxPrefab;

    [Header("Начальный лут (перезапишет инвентарь префаба)")]
    public List<InventorySlot> startingInventory = new List<InventorySlot>();

    private float timer;
    private float lifeTimer = 0f;
    private int currentSpawnedCount = 0;
    private List<GameObject> aliveUnits = new List<GameObject>();
    private bool isDepleted = false;

    void Update()
    {
        if (isDepleted) return;

        aliveUnits.RemoveAll(unit => unit == null);

        if (mode == SpawnerMode.ByTime)
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= survivalTime)
            {
                DepleteSpawner();
                return;
            }
        }

        timer += Time.deltaTime;

        bool canSpawnAlive = (maxAliveUnits == -1 || aliveUnits.Count < maxAliveUnits);

        if (timer >= spawnCooldown && canSpawnAlive)
        {
            Spawn();
            timer = 0;
        }
    }

    void Spawn()
    {
        GameObject newUnit = Instantiate(unitPrefab, transform.position, transform.rotation);
        if (container != null) newUnit.transform.SetParent(container);

        aliveUnits.Add(newUnit);
        currentSpawnedCount++;

        UnitStats stats = newUnit.GetComponent<UnitStats>();
        if (stats != null)
        {
            if (overrideOwnerID != 0) stats.ownerID = overrideOwnerID;
            if (overrideTeamID != 0) stats.teamID = overrideTeamID;

            stats.ApplyColorOptimized();
        }

        Health health = newUnit.GetComponent<Health>();
        if (health != null && stats != null)
        {
            if (spawnHealthPercent < 100f)
            {
                health.currentHealth = (int)(stats.maxHealth * (spawnHealthPercent / 100f));
            }
            else
            {
                health.currentHealth = stats.maxHealth;
            }
        }

        UnitAI ai = newUnit.GetComponent<UnitAI>();
        if (ai != null) ai.currentBehavior = spawnBehavior;
        
        newUnit.layer = LayerMask.NameToLayer("Unit");

        UnitInventory inv = newUnit.GetComponent<UnitInventory>();
        if (inv != null && startingInventory.Count > 0)
        {
            inv.slots.Clear();
            foreach (var item in startingInventory)
            {
                inv.AddResource(item.itemType, item.amount);
            }
        }

        if (mode == SpawnerMode.ByCount && totalSpawnLimit != -1 && currentSpawnedCount >= totalSpawnLimit)
        {
            DepleteSpawner();
        }
    }

    void DepleteSpawner()
    {
        isDepleted = true;
        Debug.Log($"<color=orange>[Спавнер] {gameObject.name} закончил свою работу!</color>");

        if (rewardLootBoxPrefab != null)
            Instantiate(rewardLootBoxPrefab, transform.position, Quaternion.identity);

        enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawCube(transform.position + Vector3.up * 1f, new Vector3(1f, 2f, 1f));
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + Vector3.up * 1f, transform.forward * 3f);
    }
}