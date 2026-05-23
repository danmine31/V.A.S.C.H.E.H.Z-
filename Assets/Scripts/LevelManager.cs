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
    public bool winOnTowersDestroyed = false;

    private bool isGameOver = false;
    private bool playerHadUnits = false;

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

        var playerUnits = Object.FindObjectsByType<UnitStats>(FindObjectsInactive.Exclude)
                            .Where(s => s.teamID == 1 && s.GetComponent<NavMeshAgent>() != null);

        int aliveCount = playerUnits.Count();

        if (aliveCount > 0) playerHadUnits = true;

        if (playerHadUnits && aliveCount == 0)
        {
            GameOverLose();
            return;
        }

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

    public void RegisterPlayerUnit() { }
    public void UnregisterPlayerUnit() { }

    public void GameOverWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        if (winPanel) winPanel.SetActive(true);
        Debug.Log("<color=green>ПОБЕДА!</color>");
    }

    public void GameOverLose()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        if (losePanel != null) losePanel.SetActive(true);
        Debug.Log("<color=red>ПОРАЖЕНИЕ!</color>");
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