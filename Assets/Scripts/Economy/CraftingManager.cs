using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void CraftAmmo()
    {
        UnitInventory inv = GetActiveInventory();
        if (inv == null) return;

        if (inv.GetItemCount(ItemType.Danilit) >= 2 && inv.GetItemCount(ItemType.Artemit) >= 1)
        {
            inv.RemoveItem(ItemType.Danilit, 2);
            inv.RemoveItem(ItemType.Artemit, 1);
            
            inv.AddResource(ItemType.Ammo, 5);
            
            Debug.Log("<color=green>[КРАФТ] Создано 5 патронов!</color>");
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("[КРАФТ] Не хватает ресурсов для патронов! Нужно: 2 Данилита, 1 Артемит.");
        }
    }

    public void CraftMedkit()
    {
        UnitInventory inv = GetActiveInventory();
        if (inv == null) return;

        if (inv.GetItemCount(ItemType.Egorit) >= 3 && inv.GetItemCount(ItemType.Artemit) >= 1)
        {
            inv.RemoveItem(ItemType.Egorit, 3);
            inv.RemoveItem(ItemType.Artemit, 1);
            
            inv.AddResource(ItemType.Medkit, 1);
            
            Debug.Log("<color=green>[КРАФТ] Создана 1 аптечка!</color>");
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("[КРАФТ] Не хватает ресурсов для аптечки! Нужно: 3 Егорита, 1 Артемит.");
        }
    }

    private UnitInventory GetActiveInventory()
    {
        if (SelectionController.Instance == null) return null;
        
        EntityController activeCtrl = SelectionController.Instance.GetMainSelectedController();
        if (activeCtrl != null)
        {
            return activeCtrl.GetComponent<UnitInventory>();
        }
        return null;
    }

    private void UpdateUI()
    {
        InventoryUI invUI = FindAnyObjectByType<InventoryUI>();
        if (invUI != null && invUI.IsInventoryOpen) invUI.UpdateUI();
    }
}