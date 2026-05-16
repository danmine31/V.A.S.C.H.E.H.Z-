using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Ссылки на интерфейс")]
    public GameObject inventoryPanel;

    private bool isInventoryOpen = false;

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
        Debug.Log("Инвентарь открыт! Отрисовка UI...");
    }
}