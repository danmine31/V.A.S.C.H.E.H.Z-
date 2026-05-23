using UnityEngine;
using System;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance; 
    
    public static event Action OnLanguageChanged;

    public int currentLanguage = 0;

    void Awake()
    {
        if (Instance == null) { 
            Instance = this; 
            DontDestroyOnLoad(gameObject);
        } 
        else { 
            Destroy(gameObject); 
        }

        currentLanguage = PlayerPrefs.GetInt("Language", 0);
    }

    public string GetText(string key)
    {
        if (currentLanguage == 0)
        {
            switch (key)
            {
                case "btn_play": return "Играть";
                case "btn_settings": return "Настройки";
                case "btn_quit": return "Выход";
                case "btn_back": return "Назад";
                case "lbl_volume": return "Общая громкость";
                case "lbl_lang": return "Язык";
                case "win_text": return "Уровень пройден!";
                case "lose_text": return "Поражение!";
                case "btn_restart": return "Пройти уровень снова";
                case "btn_menu": return "В главное меню";
                case "level_first": return "Уровень 1";
                case "level_second": return "Уровень 2";
                case "btn_continue": return "Продолжить";
                case "bonus_level": return "Бонусный уровень";
                case "volume": return "Громкость";
                default: return key;
            }
        }
        else
        {
            switch (key)
            {
                case "btn_play": return "Play";
                case "btn_settings": return "Settings";
                case "btn_quit": return "Quit";
                case "btn_back": return "Back";
                case "lbl_volume": return "Master Volume";
                case "lbl_lang": return "Language";
                case "win_text": return "Level completed!";
                case "lose_text": return "Defeat!";
                case "btn_restart": return "Restart";
                case "btn_menu": return "Main Menu";
                case "level_first": return "Level 1";
                case "level_second": return "Level 2";
                case "btn_continue": return "Continue";
                case "bonus_level": return "Bonus level";
                case "volume": return "Volume";
                default: return key;
            }
        }
    }

    public void ChangeLanguage(int index)
    {
        currentLanguage = index;
        PlayerPrefs.SetInt("Language", currentLanguage);
        PlayerPrefs.Save();
        
        OnLanguageChanged?.Invoke(); 
    }
}