using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [Tooltip("Ключ для перевода (например: btn_play)")]
    public string key; 
    
    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        
        LocalizationManager.OnLanguageChanged += UpdateText;
        
        UpdateText();
    }

    void OnDestroy()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    void UpdateText()
    {
        if (LocalizationManager.Instance != null && !string.IsNullOrEmpty(key))
        {
            textComponent.text = LocalizationManager.Instance.GetText(key);
        }
    }
}