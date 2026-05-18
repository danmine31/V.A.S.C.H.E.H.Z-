using UnityEngine;
using System.Collections.Generic;

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
        if (floorPrefab != null)
        {
            float floorPosX = (width * cellSize) / 2f - (cellSize / 2f);
            float floorPosZ = (height * cellSize) / 2f - (cellSize / 2f);
            GameObject floor = Instantiate(floorPrefab, new Vector3(floorPosX, 0, floorPosZ), Quaternion.identity);
            
            floor.transform.localScale = new Vector3((width * cellSize) / 10f, 1, (height * cellSize) / 10f);
            floor.transform.parent = this.transform;
            floor.layer = LayerMask.NameToLayer("Ground");
            
            if (floor.GetComponent<Collider>() == null)
            {
                floor.AddComponent<BoxCollider>();
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
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        if (enemyPrefab == null) return;

        // 1. Создаем список для хранения всех пустых координат
        List<Vector2Int> emptyCells = new List<Vector2Int>();

        // 2. Проходимся по всему лабиринту и ищем проходы (нули)
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (maze[x, y] == 0)
                {
                    // Исключаем стартовую клетку игрока (1, 1), чтобы враг не убил его сразу
                    if (x == 1 && y == 1) continue; 
                    
                    emptyCells.Add(new Vector2Int(x, y));
                }
            }
        }

        for (int i = 0; i < enemiesCount; i++)
        {
            if (emptyCells.Count == 0) break;

            int randomIndex = Random.Range(0, emptyCells.Count);
            Vector2Int cell = emptyCells[randomIndex];
            
            emptyCells.RemoveAt(randomIndex); 

            Vector3 spawnPosition = new Vector3(cell.x * cellSize, 1.0f, cell.y * cellSize);

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.transform.parent = this.transform; 
        }
    }

}