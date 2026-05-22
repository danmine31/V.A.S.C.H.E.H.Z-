using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI Экраны")]
    public GameObject winPanel;
    public GameObject losePanel;

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

    public void RegisterPlayerUnit()
    {
        alivePlayerUnits++;
    }

    public void GameOverWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        
        Time.timeScale = 0f;
        if (winPanel) winPanel.SetActive(true);
        Debug.Log("Уровень пройден!");
    }

    public void UnregisterPlayerUnit()
    {
        alivePlayerUnits--;
        Debug.Log("Осталось живых наших юнитов: " + alivePlayerUnits);
        
        if (alivePlayerUnits <= 0 && !isGameOver)
        {
            GameOverLose();
        }
    }

    public void GameOverLose()
    {
        if (isGameOver) return;
        isGameOver = true;
        
        Debug.Log("ВЫЗВАН ЭКРАН ПОРАЖЕНИЯ!");
        Time.timeScale = 0f;
        
        if (losePanel != null) 
        {
            losePanel.SetActive(true);
            Debug.Log("Панель поражения успешно включена в коде!");
        }
        else 
        {
            Debug.LogError("В LevelManager пустая ячейка LosePanel! Ты забыл перетащить её в Инспекторе!");
        }
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