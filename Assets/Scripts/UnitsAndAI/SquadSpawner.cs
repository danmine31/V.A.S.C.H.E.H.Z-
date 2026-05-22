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
    
    [Tooltip("Оставь 0, чтобы отряд использовал ID из своего префаба.")]
    public int overrideOwnerID = 0;
    public int overrideTeamID = 0;

    void Start()
    {
        for (int i = 0; i < squadSize; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            GameObject newUnit = Instantiate(unitPrefab, spawnPos, transform.rotation);
            
            if (container != null) newUnit.transform.SetParent(container);

            UnitStats stats = newUnit.GetComponent<UnitStats>();
            if (stats != null)
            {
                if (overrideOwnerID != 0) stats.ownerID = overrideOwnerID;
                if (overrideTeamID != 0) stats.teamID = overrideTeamID;
                stats.ApplyColorOptimized();
            }

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