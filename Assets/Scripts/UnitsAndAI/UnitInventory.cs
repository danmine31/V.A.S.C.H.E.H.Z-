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
    public static event System.Action OnInventoryChanged;
    [Header("Настройки инвентаря")]
    public int maxSlots = 6;
    public int maxStackSize = 44;
    [Header("Содержимое")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    public bool IsFull => slots.Count >= maxSlots;

    private void OnValidate()
    {
        if (slots != null)
        {
            if (slots.Count > maxSlots)
            {
                slots.RemoveRange(maxSlots, slots.Count - maxSlots);
            }

            foreach (var slot in slots)
            {
                if (slot.amount > maxStackSize)
                {
                    slot.amount = maxStackSize;
                }
            }
        }
    }

    public void AddResource(ItemType type, int amount)
    {
        bool isRawResource = (type == ItemType.Danilit || type == ItemType.Artemit || type == ItemType.Egorit);
        bool hasStackAlready = false;

        foreach (var slot in slots)
        {
            if (slot.itemType == type)
            {
                hasStackAlready = true;
                if (slot.amount < maxStackSize)
                {
                    int spaceLeft = maxStackSize - slot.amount;
                    int amountToAdd = Mathf.Min(amount, spaceLeft);
                    slot.amount += amountToAdd;
                    
                    OnInventoryChanged?.Invoke(); 
                    return; 
                }
            }
        }

        if (isRawResource && hasStackAlready)
        {
            Debug.LogWarning($"<color=yellow>Рабочий может нести только 1 стак {type}!</color>");
            return;
        }

        if (slots.Count < maxSlots)
        {
            slots.Add(new InventorySlot { itemType = type, amount = amount });
            OnInventoryChanged?.Invoke(); 
        }
    }

    public bool RemoveItem(ItemType type, int amountToRemove)
    {
        if (GetItemCount(type) < amountToRemove) return false;

        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].itemType == type)
            {
                if (slots[i].amount >= amountToRemove)
                {
                    slots[i].amount -= amountToRemove;
                    if (slots[i].amount <= 0) slots.RemoveAt(i);
                    return true;
                }
                else
                {
                    amountToRemove -= slots[i].amount;
                    slots.RemoveAt(i);
                }
            }
        }
        return true;
    }

    public int GetItemCount(ItemType type)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (slot.itemType == type) total += slot.amount;
        }
        return total;
    }

    public bool CanAcceptItem(ItemType type)
    {
        foreach (var slot in slots)
        {
            if (slot.itemType == type && slot.amount < maxStackSize) return true;
        }
        return slots.Count < maxSlots;
    }
}