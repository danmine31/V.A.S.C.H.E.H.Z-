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
    public TextMeshProUGUI entityNameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI ammoText; 
    public TextMeshProUGUI medkitText;
    public TextMeshProUGUI influenceText;

    [Header("Кнопки приказов")]
    public Button inventoryButton;
    public Button healButton;

    void Awake() { if (Instance == null) Instance = this; }

    void Update()
    {
        UpdateHUD();
        if (Input.GetKeyDown(KeyCode.X)) OnHealButtonClicked();
    }

    void UpdateHUD()
    {
        if (SelectionController.Instance == null) return;

        if (influenceText != null && GameManager.Instance != null)
            influenceText.text = $"<color=orange>Влияние: {GameManager.Instance.influencePoints}</color>";
        
        EntityController selectedController = SelectionController.Instance.GetMainSelectedController();
        EntityStats inspectedEntity = SelectionController.Instance.GetInspectedEntity();

        bool isOwnUnit = (selectedController != null);

        EntityStats displayStats = selectedController != null ? selectedController.GetComponent<EntityStats>() : inspectedEntity;

        if (displayStats != null)
        {
            if (bottomHUD != null) bottomHUD.SetActive(true);

            Health health = displayStats.GetComponent<Health>();
            UnitInventory inventory = displayStats.GetComponent<UnitInventory>();
            WeaponComponent weapon = displayStats.GetComponent<WeaponComponent>();

            if (portraitImage != null) portraitImage.enabled = true;

            if (health != null)
            {
                if (entityNameText != null)
                {
                    int selectedCount = SelectionController.Instance.GetSelectedControllers().Count;
                    if (selectedCount > 1)
                        entityNameText.text = $"{displayStats.entityName}\n<color=yellow><size=70%>ВЫДЕЛЕНО: {selectedCount}</size></color>";
                    else 
                        entityNameText.text = displayStats.entityName;
                }

                if (statsText != null) 
                {
                    string weaponStats = weapon != null 
                        ? $"Урон: {weapon.minDamage}-{weapon.maxDamage} | Пробой: {weapon.armorPenetration}\nКрит: {weapon.critChance}% (x{weapon.critMultiplier})\nДальность: {weapon.attackRange}"
                        : "Оружие: Нет (Рабочий)";

                    if (displayStats is UnitStats uStats)
                    {
                        statsText.text = 
                            $"Уровень: {uStats.level} (Опыт: {Mathf.Round(uStats.currentXP)} / {Mathf.Round(uStats.GetXPForNextLevel())})\n" +
                            $"ХП: {Mathf.Round(health.currentHealth)} / {uStats.maxHealth}\n" +
                            weaponStats + "\n" +
                            $"Броня: {uStats.armor} | Уворот: {uStats.dodgeChance}%";
                    }
                    else
                    {
                        string buildWpnStats = weapon != null ? $"\nУрон: {weapon.minDamage}-{weapon.maxDamage} | Дальность: {weapon.attackRange}" : "";
                        statsText.text = $"ХП: {Mathf.Round(health.currentHealth)} / {displayStats.maxHealth}\nБроня: {displayStats.armor}" + buildWpnStats;
                    }
                }
            }

            if (inventory != null)
            {
                int totalAmmo = 0, totalMedkits = 0;
                foreach (var ctrl in SelectionController.Instance.GetSelectedControllers())
                {
                    if (ctrl == null) continue;
                    UnitInventory inv = ctrl.GetComponent<UnitInventory>();
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
            if (bottomHUD != null) bottomHUD.SetActive(false);
            if (portraitImage != null) portraitImage.enabled = false;
            if (entityNameText != null) entityNameText.text = "";
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
        foreach (var ctrl in SelectionController.Instance.GetSelectedControllers())
        {
            if (ctrl != null)
            {
                Health h = ctrl.GetComponent<Health>();
                if (h != null && h.currentHealth < h.maxHealth) h.TryStartHealing();
            }
        }
    }
}