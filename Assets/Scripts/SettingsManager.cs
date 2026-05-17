using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        Debug.Log("Громкость изменена на: " + volume);
    }

    public void SetLanguage(int languageIndex)
    {
        Debug.Log("Выбран язык: " + languageIndex);
        
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.ChangeLanguage(languageIndex);
        }
    }
}