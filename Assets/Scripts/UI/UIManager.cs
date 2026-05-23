using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Панели")]
    public GameObject bottomHUD;

    [Header("Портрет и Тексты")]
    public RawImage portraitImage;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI ammoText; 
    public TextMeshProUGUI medkitText;
    public TextMeshProUGUI influenceText;

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

        if (Input.GetKeyDown(KeyCode.X))
        {
            OnHealButtonClicked();
        }
    }

    void UpdateHUD()
    {
        if (SelectionController.Instance == null) return;

        if (influenceText != null && GameManager.Instance != null)
        {
            influenceText.text = $"<color=orange>Влияние: {GameManager.Instance.influencePoints}</color>";
        }
        
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
                if (unitNameText != null)
                {
                    int selectedCount = SelectionController.Instance.GetSelectedUnits().Count;
                    if (selectedCount > 1)
                    {
                        unitNameText.text = $"{displayStats.unitName}\n<color=yellow><size=70%>ВЫДЕЛЕНО: {selectedCount}</size></color>";
                    }
                    else unitNameText.text = displayStats.unitName;
                }

                if (statsText != null) 
                {
                    statsText.text = 
                        $"Уровень: {displayStats.level} (Опыт: {Mathf.Round(displayStats.currentXP)} / {Mathf.Round(displayStats.GetXPForNextLevel())})\n" +
                        $"ХП: {Mathf.Round(health.currentHealth)} / {displayStats.maxHealth}\n" +
                        $"Урон: {displayStats.minDamage}-{displayStats.maxDamage} | Пробой: {displayStats.armorPenetration}\n" +
                        $"Крит: {displayStats.critChance}% (x{displayStats.critMultiplier})\n" +
                        $"Броня: {displayStats.armor} | Уворот: {displayStats.dodgeChance}%\n" +
                        $"Дальность: {displayStats.attackRange}";
                }
            }

            if (inventory != null)
            {
                int totalAmmo = 0;
                int totalMedkits = 0;

                foreach (var unit in SelectionController.Instance.GetSelectedUnits())
                {
                    if (unit == null) continue;

                    UnitInventory inv = unit.GetComponent<UnitInventory>();
                    if (inv != null)
                    {
                        totalAmmo += inv.GetItemCount(ItemType.Ammo);
                        totalMedkits += inv.GetItemCount(ItemType.Medkit);
                    }
                }
                
                if (isOwnUnit)
                {
                    if (ammoText != null) ammoText.text = $"<color=yellow>Патроны: {totalAmmo}</color>";
                    if (medkitText != null) medkitText.text = $"<color=#00BFFF>Аптечки: {totalMedkits}</color>";
                }
                else
                {
                    if (ammoText != null) ammoText.text = "";
                    if (medkitText != null) medkitText.text = "";
                }
                
                if (healButton != null) healButton.interactable = (isOwnUnit && totalMedkits > 0);
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

    public void OnInventoryButtonClicked()
    {
        InventoryUI invUI = FindAnyObjectByType<InventoryUI>();
        if (invUI != null) invUI.ToggleInventory();
    }

    public void OnHealButtonClicked()
    {
        if (SelectionController.Instance == null) return;
        foreach (var unit in SelectionController.Instance.GetSelectedUnits())
        {
            if (unit != null)
            {
                Health h = unit.GetComponent<Health>();
                if (h != null && h.currentHealth < h.maxHealth) h.TryStartHealing();
            }
        }
    }
}