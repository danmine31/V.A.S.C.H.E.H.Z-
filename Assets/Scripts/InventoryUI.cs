using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public struct ItemIcon
{
    public ItemType itemType;
    public Sprite icon;
}

public class InventoryUI : MonoBehaviour
{
    [Header("Настройки UI")]
    public GameObject inventoryPanel;
    public Transform slotContainer;
    public GameObject slotPrefab;

    [Header("База иконок")]
    public List<ItemIcon> itemDatabase;

    public bool IsInventoryOpen { get; private set; }

    private SelectionController selectionCtrl;
    private UnitController lastSelectedUnit;

    void Start()
    {
        selectionCtrl = FindObjectOfType<SelectionController>();
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        IsInventoryOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        if (IsInventoryOpen)
        {
            UnitController currentUnit = selectionCtrl != null ? selectionCtrl.GetMainSelectedUnit() : null;

            if (currentUnit == null)
            {
                CloseInventory();
                return;
            }

            if (currentUnit != lastSelectedUnit)
            {
                UpdateUI();
            }
        }
    }

    public void ToggleInventory()
    {
        if (IsInventoryOpen)
        {
            CloseInventory();
        }
        else
        {
            if (selectionCtrl != null && selectionCtrl.GetMainSelectedUnit() == null)
            {
                Debug.LogWarning("Сначала выделите юнита!");
                return; 
            }
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        if (IsInventoryOpen) return;
        IsInventoryOpen = true;
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
        UpdateUI();
    }

    public void CloseInventory()
    {
        if (!IsInventoryOpen) return;
        IsInventoryOpen = false;
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        lastSelectedUnit = null;

        LootUI lootUI = FindObjectOfType<LootUI>();
        if (lootUI != null)
        {
            lootUI.CloseLoot();
        }
    }

    public void UpdateUI()
    {
        if (selectionCtrl == null) return;
        UnitController activeUnit = selectionCtrl.GetMainSelectedUnit();
        if (activeUnit == null) return;

        UnitInventory inv = activeUnit.GetComponent<UnitInventory>();
        if (inv == null) return;

        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < inv.maxSlots; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotContainer);

            SlotUI slotScript = newSlot.GetComponent<SlotUI>();
            if (slotScript != null)
            {
                slotScript.panelType = SlotPanelType.Inventory;
                slotScript.slotIndex = i;
            }
            
            Image iconImage = newSlot.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI amountText = newSlot.transform.Find("AmountText").GetComponent<TextMeshProUGUI>();

            if (i < inv.slots.Count)
            {
                var slotData = inv.slots[i];
                
                Sprite foundIcon = GetIcon(slotData.itemType);
                if (foundIcon != null)
                {
                    iconImage.sprite = foundIcon;
                    iconImage.color = Color.white;
                }

                amountText.text = slotData.amount.ToString();
            }
            else
            {
                iconImage.color = new Color(0, 0, 0, 0);
                amountText.text = "";
            }
        }

        lastSelectedUnit = activeUnit;
    }

    private Sprite GetIcon(ItemType type)
    {
        foreach (var item in itemDatabase)
        {
            if (item.itemType == type) return item.icon;
        }
        return null;
    }
}