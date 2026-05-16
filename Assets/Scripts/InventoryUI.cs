using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Ссылки на интерфейс")]
    public GameObject inventoryPanel;

    private bool isInventoryOpen = false;
    private SelectionController selectionCtrl;

    void Start()
    {
        selectionCtrl = FindFirstObjectByType<SelectionController>();
        
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        if (selectionCtrl == null) return;

        UnitController activeUnit = selectionCtrl.GetMainSelectedUnit();

        if (activeUnit != null)
        {
            UnitInventory inv = activeUnit.GetComponent<UnitInventory>();
            if (inv != null)
            {
                Debug.Log($"<color=green>Открыт рюкзак юнита: {activeUnit.gameObject.name}</color>");
                Debug.Log($"Занято слотов: {inv.slots.Count} из {inv.maxSlots}");
            }
            else
            {
                Debug.LogWarning("У этого юнита нет скрипта UnitInventory!");
            }
        }
        else
        {
            Debug.Log("Нет выделенного юнита! Инвентарь пуст.");
        }
    }
}