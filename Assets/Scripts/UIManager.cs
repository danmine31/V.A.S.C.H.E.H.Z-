using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Панели")]
    public GameObject bottomHUD;

    [Header("Портрет и Тексты")]
    public Image portraitImage; 
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI ammoText; 
    public TextMeshProUGUI medkitText;

    [Header("Кнопки приказов")]
    public Button inventoryButton;
    public Button healButton;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (SelectionController.Instance == null) return;
        
        UnitController selectedUnit = SelectionController.Instance.GetMainSelectedUnit();

        if (bottomHUD != null && !bottomHUD.activeSelf) 
            bottomHUD.SetActive(true);

        if (selectedUnit != null)
        {
            UnitStats stats = selectedUnit.GetComponent<UnitStats>();
            Health health = selectedUnit.GetComponent<Health>();
            UnitInventory inventory = selectedUnit.GetComponent<UnitInventory>();

            if (portraitImage != null) portraitImage.enabled = true;

            if (stats != null && health != null)
            {
                if (unitNameText != null) unitNameText.text = stats.unitName;
                if (statsText != null) statsText.text = $"ХП: {Mathf.Round(health.currentHealth)} / {stats.maxHealth}\nУрон: {stats.damage}";
            }

            if (inventory != null)
            {
                int ammoCount = inventory.GetItemCount(ItemType.Ammo);
                int medkitCount = inventory.GetItemCount(ItemType.Medkit);
                
                if (ammoText != null) ammoText.text = $"Патроны: {ammoCount}";
                if (medkitText != null) medkitText.text = $"Аптечки: {medkitCount}";
                
                if (healButton != null) healButton.interactable = (medkitCount > 0);
                if (inventoryButton != null) inventoryButton.interactable = true;
            }
            else
            {
                if (ammoText != null) ammoText.text = "";
                if (medkitText != null) medkitText.text = "";
                if (healButton != null) healButton.interactable = false;
                if (inventoryButton != null) inventoryButton.interactable = false;
            }
        }
        else
        {
            if (portraitImage != null) portraitImage.enabled = false;
            if (unitNameText != null) unitNameText.text = "";
            if (statsText != null) statsText.text = "";
            if (ammoText != null) ammoText.text = "";
            if (medkitText != null) medkitText.text = "";

            if (inventoryButton != null) inventoryButton.interactable = false;
            if (healButton != null) healButton.interactable = false;
        }
    }
}