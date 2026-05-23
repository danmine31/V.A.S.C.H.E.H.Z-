using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FactionSetup
{
    public string factionName;
    public int teamID;
    public int ownerID;
    public GameObject[] availableUnits;
    public int unitsToSpawn = 5;
}

public class CustomMapGenerator : MonoBehaviour
{
    public static CustomMapGenerator Instance;

    [Header("Основа уровня")]
    public Transform groundPlane;
    public Vector2 mapSize = new Vector2(200f, 200f);

    [Header("Фракции на карте (Добавь Игрока и Бота)")]
    public List<FactionSetup> factions = new List<FactionSetup>();

    [Header("Префабы окружения")]
    public GameObject[] structurePrefabs; 
    public GameObject[] resourcePrefabs;  

    [Header("Ползунки Плотности")]
    [Range(0, 100)] public int structureCount = 30; 
    [Range(0, 50)] public int resourceCount = 15;   
    
    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Awake() { if (Instance == null) Instance = this; }

    public void GenerateMap()
    {
        ClearMap();

        if (groundPlane != null)
            groundPlane.localScale = new Vector3(mapSize.x / 10f, 1f, mapSize.y / 10f);

        ScatterObjects(structurePrefabs, structureCount);
        ScatterObjects(resourcePrefabs, resourceCount);

        Vector3[] spawnCorners = new Vector3[] {
            new Vector3(-mapSize.x / 3f, 1f, -mapSize.y / 3f),
            new Vector3(mapSize.x / 3f, 1f, mapSize.y / 3f),
            new Vector3(-mapSize.x / 3f, 1f, mapSize.y / 3f),
            new Vector3(mapSize.x / 3f, 1f, -mapSize.y / 3f)
        };

        for (int i = 0; i < factions.Count; i++)
        {
            if (i < spawnCorners.Length)
            {
                SpawnFactionUnits(factions[i], spawnCorners[i]);
            }
        }

        UpdateCameras();
    }

    public void ClearMap()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }

    void ScatterObjects(GameObject[] prefabs, int count)
    {
        if (prefabs == null || prefabs.Length == 0) return;
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-mapSize.x / 2.2f, mapSize.x / 2.2f), 50f, Random.Range(-mapSize.y / 2.2f, mapSize.y / 2.2f));
            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 100f))
            {
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                GameObject newObj = Instantiate(prefab, hit.point, Quaternion.Euler(0, Random.Range(0, 360), 0), this.transform);
                spawnedObjects.Add(newObj);
            }
        }
    }

    void SpawnFactionUnits(FactionSetup faction, Vector3 basePos)
    {
        if (faction.availableUnits == null || faction.availableUnits.Length == 0) return;

        for (int i = 0; i < faction.unitsToSpawn; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * 10f; 
            Vector3 spawnPos = basePos + new Vector3(randomCircle.x, 50f, randomCircle.y);

            if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hit, 100f))
            {
                GameObject prefab = faction.availableUnits[Random.Range(0, faction.availableUnits.Length)];
                GameObject newUnit = Instantiate(prefab, hit.point, Quaternion.identity, this.transform);
                spawnedObjects.Add(newUnit);

                UnitStats stats = newUnit.GetComponent<UnitStats>();
                if (stats != null)
                {
                    stats.teamID = faction.teamID;
                    stats.ownerID = faction.ownerID;
                }
            }
        }
    }

    void UpdateCameras()
    {
        CameraController cam = FindAnyObjectByType<CameraController>();
        if (cam != null && groundPlane != null)
        {
            cam.groundRenderer = groundPlane.GetComponent<Renderer>();
            cam.UpdateMapBounds();
        }
    }
}