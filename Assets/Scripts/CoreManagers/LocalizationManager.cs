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