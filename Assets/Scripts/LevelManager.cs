using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI Экраны")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Условия победы")]
    [Tooltip("Если включено, победа наступит при уничтожении всех вражеских башен")]
    public bool winOnTowersDestroyed = false;

    private int alivePlayerUnits = 0;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        if (winOnTowersDestroyed)
        {
            var enemyTowers = Object.FindObjectsByType<UnitStats>(FindObjectsInactive.Exclude)
                                .Where(s => s.teamID == 2 && s.GetComponent<NavMeshAgent>() == null);

            if (!enemyTowers.Any())
            {
                GameOverWin();
            }
        }
    }

    public void RegisterPlayerUnit() { alivePlayerUnits++; }

    public void UnregisterPlayerUnit()
    {
        alivePlayerUnits--;
        if (alivePlayerUnits <= 0 && !isGameOver) GameOverLose();
    }

    public void GameOverWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        if (winPanel) winPanel.SetActive(true);
        Debug.Log("Уровень пройден!");
    }

    public void GameOverLose()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        if (losePanel != null) losePanel.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
}