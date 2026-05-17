using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LootUI : MonoBehaviour
{
    [Header("Настройки UI")]
    public GameObject lootPanel;
    public Transform slotContainer;
    public GameObject slotPrefab;

    public LootBox CurrentLootBox => currentLootBox;
    private LootBox currentLootBox;

    private InventoryUI inventoryUI;
    private SelectionController selectionCtrl;

    void Start()
    {
        inventoryUI = FindObjectOfType<InventoryUI>();
        selectionCtrl = FindObjectOfType<SelectionController>();
        
        if (lootPanel != null) lootPanel.SetActive(false);
    }

    void Update()
    {
        if (lootPanel != null && lootPanel.activeInHierarchy)
        {
            UnitController activeUnit = selectionCtrl != null ? selectionCtrl.GetMainSelectedUnit() : null;

            if (activeUnit == null || currentLootBox == null)
            {
                CloseLoot();
                return;
            }

            float distance = Vector3.Distance(activeUnit.transform.position, currentLootBox.transform.position);
            if (distance > 4f)
            {
                CloseLoot();
            }
        }
    }

    public void OpenLoot(LootBox box)
    {
        currentLootBox = box;
        if (lootPanel != null) lootPanel.SetActive(true);
        
        if (inventoryUI != null)
        {
            inventoryUI.OpenInventory();
        }

        UpdateUI();
    }

    public void CloseLoot()
    {
        if (currentLootBox == null && (lootPanel != null && !lootPanel.activeInHierarchy)) return;

        LootBox boxToClose = currentLootBox; 
        
        currentLootBox = null;
        if (lootPanel != null) lootPanel.SetActive(false);

        if (inventoryUI != null && inventoryUI.IsInventoryOpen)
        {
            inventoryUI.CloseInventory();
        }

        if (boxToClose != null)
        {
            boxToClose.CheckEmptyState();
        }
    }

    public void UpdateUI()
    {
        if (currentLootBox == null) return;

        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentLootBox.maxSlots; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotContainer);
            
            SlotUI slotScript = newSlot.GetComponent<SlotUI>();
            if (slotScript != null)
            {
                slotScript.panelType = SlotPanelType.LootBox;
                slotScript.slotIndex = i;
            }

            Image iconImage = newSlot.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI amountText = newSlot.transform.Find("AmountText").GetComponent<TextMeshProUGUI>();

            if (i < currentLootBox.boxContents.Count)
            {
                var lootItem = currentLootBox.boxContents[i];
                Sprite foundIcon = GetIconFromDatabase(lootItem.itemType);
                if (foundIcon != null)
                {
                    iconImage.sprite = foundIcon;
                    iconImage.color = Color.white;
                }
                amountText.text = lootItem.amount.ToString();
            }
            else
            {
                iconImage.color = new Color(0, 0, 0, 0);
                amountText.text = "";
            }
        }
    }

    private Sprite GetIconFromDatabase(ItemType type)
    {
        if (inventoryUI == null) return null;
        
        foreach (var item in inventoryUI.itemDatabase)
        {
            if (item.itemType == type) return item.icon;
        }
        return null;
    }
}