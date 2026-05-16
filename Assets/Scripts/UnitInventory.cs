using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventorySlot
{
    public ItemType itemType;
    public int amount;
}

public class UnitInventory : MonoBehaviour
{
    [Header("Настройки инвентаря")]
    public int maxSlots = 6;
    public int maxStackSize = 44;
    [Header("Содержимое")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    public bool IsFull => slots.Count >= maxSlots;

    public void AddResource(ItemType type, int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.itemType == type && slot.amount < maxStackSize)
            {
                int spaceLeft = maxStackSize - slot.amount;
                int amountToAdd = Mathf.Min(amount, spaceLeft);
                
                slot.amount += amountToAdd;
                Debug.Log($"Добавлено {amountToAdd} {type}. В ячейке теперь: {slot.amount}");
                return;
            }
        }

        if (slots.Count < maxSlots)
        {
            slots.Add(new InventorySlot { itemType = type, amount = amount });
            Debug.Log($"Заняли новый слот под: {type} ({amount} шт.)");
        }
        else
        {
            Debug.LogWarning("Инвентарь переполнен! Некуда класть " + type);
        }
    }
}