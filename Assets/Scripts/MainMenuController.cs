using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Настройки сцен")]
    [Tooltip("Точное название сцены, которая должна загружаться при старте игры")]
    public string gameSceneName = "Level_01";

    public void PlayGame()
    {
        Debug.Log("Загружаем игру...");
        SceneManager.LoadScene(gameSceneName);
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
}