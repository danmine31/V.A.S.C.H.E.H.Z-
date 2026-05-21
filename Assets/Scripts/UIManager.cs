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
        UnitStats inspectedUnit = SelectionController.Instance.GetInspectedUnit();

        bool isOwnUnit = (selectedUnit != null);

        UnitStats displayStats = null;
        if (selectedUnit != null) 
            displayStats = selectedUnit.GetComponent<UnitStats>();
        else 
            displayStats = SelectionController.Instance.GetInspectedUnit();

        if (bottomHUD != null && !bottomHUD.activeSelf) 
            bottomHUD.SetActive(true);

        if (displayStats != null)
        {
            Health health = displayStats.GetComponent<Health>();
            UnitInventory inventory = displayStats.GetComponent<UnitInventory>();

            if (portraitImage != null) portraitImage.enabled = true;

            if (health != null)
            {
                if (unitNameText != null) unitNameText.text = displayStats.unitName;
                if (statsText != null) statsText.text = $"ХП: {Mathf.Round(health.currentHealth)} / {displayStats.maxHealth}\nУрон: {displayStats.damage}";
            }

            if (inventory != null)
            {
                int ammoCount = inventory.GetItemCount(ItemType.Ammo);
                int medkitCount = inventory.GetItemCount(ItemType.Medkit);
                
                if (ammoText != null) ammoText.text = $"Патроны: {ammoCount}";
                if (medkitText != null) medkitText.text = $"Аптечки: {medkitCount}";
                
                if (healButton != null) healButton.interactable = (isOwnUnit && medkitCount > 0);
                if (inventoryButton != null) inventoryButton.interactable = isOwnUnit;
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