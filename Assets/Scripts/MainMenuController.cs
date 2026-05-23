using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Панели (Объекты из сцены)")]
    [Tooltip("Панель со стартовыми кнопками: Играть, Настройки, Выход")]
    public GameObject mainMenuPanel;

    [Tooltip("Панель со списком уровней: Лабиринт, Погода, Назад")]
    public GameObject levelSelectionPanel;

    void Start()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(false);
    }

    public void PlayGame()
    {
        Debug.Log("Открываем окно выбора уровней...");
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(true);
    }

    public void CloseLevelSelection()
    {
        Debug.Log("Возвращаемся в главное меню...");
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(false);
    }

    public void LoadLevel(string levelName)
    {
        Debug.Log($"Загружаем уровень: {levelName}");
        SceneManager.LoadScene(levelName);
    }

    public void OpenSettings()
    {
        Debug.Log("Открыто меню настроек");
    }

    public void QuitGame()
    {
        Debug.Log("Выходим из игры...");
        Application.Quit(); 
    }

    public void LoadCustomLevel()
    {
        SceneManager.LoadScene("Level_Custom");
    }
}