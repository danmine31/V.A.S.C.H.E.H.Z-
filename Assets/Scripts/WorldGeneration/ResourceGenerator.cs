using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    [Header("Префабы ресурсов")]
    public GameObject[] resourcePrefabs;

    [Header("Настройки генерации")]
    public int totalResourcesCount = 30;
    public Vector2 mapSize = new Vector2(250f, 250f);

    void Start()
    {
        GenerateResources();
    }

    void GenerateResources()
    {
        if (resourcePrefabs == null || resourcePrefabs.Length == 0) return;

        for (int i = 0; i < totalResourcesCount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-mapSize.x / 2, mapSize.x / 2),
                50f,
                Random.Range(-mapSize.y / 2, mapSize.y / 2)
            );

            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 100f))
            {
                GameObject prefabToSpawn = resourcePrefabs[Random.Range(0, resourcePrefabs.Length)];
                
                Instantiate(prefabToSpawn, hit.point, Quaternion.identity, this.transform);
            }
        }
        Debug.Log($"<color=cyan>[Генератор] Успешно раскидано {totalResourcesCount} ресурсов по карте!</color>");
    }
}