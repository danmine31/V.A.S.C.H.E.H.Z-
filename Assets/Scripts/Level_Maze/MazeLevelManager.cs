using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeLevelManager : MonoBehaviour
{
    [Header("Настройки уровня")]
    public GameObject playerUnit;
    public Transform exitPoint;
    public float exitRadius = 2.0f;

    private Health unitHealth;
    private bool isLevelOver = false;

    void Start()
    {
        if (playerUnit != null)
        {
            unitHealth = playerUnit.GetComponent<Health>();
        }
        else
        {
            Debug.LogError("Юнит игрока не назначен в MazeLevelManager!");
        }
    }

    void Update()
    {
        if (isLevelOver) return;

        if (unitHealth != null && unitHealth.currentHealth <= 0)
        {
            LevelFailed();
        }

        if (playerUnit != null && exitPoint != null)
        {
            float distanceToExit = Vector3.Distance(playerUnit.transform.position, exitPoint.position);
            if (distanceToExit <= exitRadius)
            {
                LevelCompleted();
            }
        }
    }

    void LevelFailed()
    {
        isLevelOver = true;
        Debug.Log("Юнит погиб. Лабиринт стал его гробницей!");
    }

    void LevelCompleted()
    {
        isLevelOver = true;
        Debug.Log("Побег удался!");
    }
}