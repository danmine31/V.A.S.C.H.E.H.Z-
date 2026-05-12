using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [Header("Настройки шаблона")]
    public GameObject unitPrefab;
    public Transform container;
    
    [Header("Настройки Фракции (Материалы)")]
    public Material playerMaterial;
    public Material enemyMaterial;
    
    [Header("Параметры спавна")]
    public float spawnCooldown = 15f;
    public bool isPlayerFaction = true; 
    
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnCooldown)
        {
            Spawn();
            timer = 0;
        }
    }

    void Spawn()
    {
        GameObject newUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        
        if (container != null) 
            newUnit.transform.SetParent(container);

        SetupFaction(newUnit);
    }

    void SetupFaction(GameObject unit)
    {
        unit.layer = LayerMask.NameToLayer(isPlayerFaction ? "Unit" : "Enemy");

        var renderer = unit.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = isPlayerFaction ? playerMaterial : enemyMaterial;
        }
    }
}