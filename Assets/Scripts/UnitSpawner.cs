using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [Header("Настройки шаблона")]
    public GameObject unitPrefab;
    public Transform container;

    [Header("Настройки ИИ")]
    public AIBehavior spawnBehavior = AIBehavior.Defend;
    
    [Header("Командные настройки")]
    public int teamID = 0;
    public int colorID = 0;
    public Material teamMaterial;

    [Header("Параметры спавна")]
    public float spawnCooldown = 15f;
    
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
        {
            newUnit.transform.SetParent(container);
        }

        Health health = newUnit.GetComponent<Health>();
        if (health != null)
        {
            health.teamID = this.teamID;
            health.colorID = this.colorID;
        }

        var renderer = newUnit.GetComponentInChildren<Renderer>();
        if (renderer != null && teamMaterial != null)
        {
            renderer.material = teamMaterial;
        }

        UnitAI ai = newUnit.GetComponent<UnitAI>();
        if (ai != null)
        {
            ai.currentBehavior = spawnBehavior;
        }
        
        newUnit.layer = LayerMask.NameToLayer("Unit");
    }
}