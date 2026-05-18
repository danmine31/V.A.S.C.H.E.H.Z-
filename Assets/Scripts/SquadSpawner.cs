using UnityEngine;

public class StorySquadSpawner : MonoBehaviour
{
    [Header("Настройки Отряда")]
    public GameObject unitPrefab;
    public int squadSize = 4;
    public float spawnRadius = 3f;
    public Transform container;

    [Header("Настройки ИИ и Команды")]
    public AIBehavior spawnBehavior = AIBehavior.Defend;
    public int teamID = 0;
    public int colorID = 0;
    public Material teamMaterial;

    void Start()
    {
        for (int i = 0; i < squadSize; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            GameObject newUnit = Instantiate(unitPrefab, spawnPos, transform.rotation);
            
            if (container != null) newUnit.transform.SetParent(container);

            Health health = newUnit.GetComponent<Health>();
            if (health != null)
            {
                health.teamID = this.teamID;
                health.colorID = this.colorID;
            }

            var renderer = newUnit.GetComponentInChildren<Renderer>();
            if (renderer != null && teamMaterial != null) renderer.material = teamMaterial;

            UnitAI ai = newUnit.GetComponent<UnitAI>();
            if (ai != null) ai.currentBehavior = spawnBehavior;
            
            newUnit.layer = LayerMask.NameToLayer("Unit");
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, spawnRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * spawnRadius);
    }
}