using UnityEngine;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class MazeGenerator : MonoBehaviour
{
    [Header("Настройки лабиринта (только нечетные числа!)")]
    public int width = 21;
    public int height = 21;
    public float cellSize = 5f;

    [Header("Префабы")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    [Header("Враги")]
    public GameObject enemyPrefab;
    public int enemiesCount = 3;

    private int[,] maze;

    void Start()
    {
        maze = new int[width, height];
    }

    void GenerateMazeLogic()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 1;

        CarvePath(1, 1);

        maze[1, 0] = 0; 
        
        maze[width - 2, height - 1] = 0; 
    }

    void CarvePath(int x, int y)
    {
        maze[x, y] = 0;

        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(0, 2), new Vector2Int(0, -2),
            new Vector2Int(2, 0), new Vector2Int(-2, 0)
        };
        
        Shuffle(directions);

        foreach (Vector2Int dir in directions)
        {
            int nextX = x + dir.x;
            int nextY = y + dir.y;

            if (nextX > 0 && nextX < width - 1 && nextY > 0 && nextY < height - 1 && maze[nextX, nextY] == 1)
            {
                maze[x + dir.x / 2, y + dir.y / 2] = 0;
                CarvePath(nextX, nextY);
            }
        }
    }

    void BuildMazeInScene() 
    {
        GameObject floorObj = null;

        if (floorPrefab != null) 
        {
            float floorPosX = (width * cellSize) / 2f - (cellSize / 2f);
            float floorPosZ = (height * cellSize) / 2f - (cellSize / 2f);
            floorObj = Instantiate(floorPrefab, new Vector3(floorPosX, 0, floorPosZ), Quaternion.identity);
            
            float floorMargin = 5f; // Наш отступ
            floorObj.transform.localScale = new Vector3(
                ((width * cellSize) / 10f) + floorMargin, 
                1, 
                ((height * cellSize) / 10f) + floorMargin
            );
            floorObj.transform.parent = this.transform;
            floorObj.layer = LayerMask.NameToLayer("Ground");
            
            if (floorObj.GetComponent<Collider>() == null) {
                floorObj.AddComponent<BoxCollider>();
            }
        }

        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++) 
            {
                if (maze[x, y] == 1) 
                {
                    float wallHeight = wallPrefab.transform.localScale.y;
                    Vector3 position = new Vector3(x * cellSize, wallHeight / 2f, y * cellSize);
                    GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
                    wall.transform.parent = this.transform;
                }
            }
        }

        if (floorObj != null)
        {
            NavMeshSurface navSurface = floorObj.GetComponent<NavMeshSurface>();
            if (navSurface == null)
            {
                navSurface = floorObj.AddComponent<NavMeshSurface>();
            }
            
            navSurface.BuildNavMesh();
        }

        int enemySpawnAttempts = 0;
        while (enemySpawnAttempts < 5)
        {
            int rx = Random.Range(0, width);
            int ry = Random.Range(0, height);

            if (maze[rx, ry] == 0)
            {
                Vector3 spawnPos = new Vector3(rx * cellSize, 1f, ry * cellSize);
                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                enemySpawnAttempts++;
            }
        }
    }

    void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int tempIndex = Random.Range(i, list.Count);
            Vector2Int temp = list[i];
            list[i] = list[tempIndex];
            list[tempIndex] = temp;
        }
    }
    [ContextMenu("Сгенерировать лабиринт сейчас")]
    public void BuildMazeInEditor()
    {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        maze = new int[width, height];
        GenerateMazeLogic();
        BuildMazeInScene();
    }
}