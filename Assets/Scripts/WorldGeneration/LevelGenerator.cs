using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    [Header("Префабы для генерации")]
    public GameObject playerSpawnerPrefab;
    public GameObject enemySpawnerPrefab;
    public GameObject buildingPrefab;

    [Header("Количество объектов")]
    public int playerSpawnerCount = 3;
    public int enemySpawnerCount = 3;
    public int buildingCount = 10;

    [Header("Настройки зоны генерации")]
    public Vector2 mapSize = new Vector2(40f, 40f);
    public LayerMask groundLayer;

    void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        SpawnObjects(buildingPrefab, buildingCount);
        
        SpawnObjects(playerSpawnerPrefab, playerSpawnerCount);
        SpawnObjects(enemySpawnerPrefab, enemySpawnerCount);
    }

    void SpawnObjects(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = GetValidSpawnPosition();
            
            if (spawnPos != Vector3.zero)
            {
                Instantiate(prefab, spawnPos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"Не удалось найти свободное место для объекта {prefab.name}");
            }
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        for (int i = 0; i < 30; i++) 
        {
            float randomX = Random.Range(-mapSize.x / 2, mapSize.x / 2);
            float randomZ = Random.Range(-mapSize.y / 2, mapSize.y / 2);
            
            Vector3 rayStart = new Vector3(transform.position.x + randomX, 100f, transform.position.z + randomZ);
            
            RaycastHit hit;
            if (Physics.Raycast(rayStart, Vector3.down, out hit, 200f, groundLayer))
            {
                NavMeshHit navHit;
                if (NavMesh.SamplePosition(hit.point, out navHit, 2.0f, NavMesh.AllAreas))
                {
                    return navHit.position;
                }
            }
        }
        
        return Vector3.zero; 
    }
}