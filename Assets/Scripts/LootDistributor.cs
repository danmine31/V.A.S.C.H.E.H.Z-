using UnityEngine;
using System.Collections.Generic;

public static class LootDistributor
{
    public static void DistributeAmongSquad(List<UnitController> squad, ItemType item, int totalAmount)
    {
        int remainingAmount = totalAmount;
        
        bool isConsumable = (item == ItemType.Ammo || item == ItemType.Medkit || 
                             item == ItemType.Artemit || item == ItemType.Danilit || item == ItemType.Egorit);

        while (remainingAmount > 0)
        {
            UnitInventory bestTarget = null;

            if (isConsumable)
            {
                int minAmount = int.MaxValue;
                foreach (var unit in squad)
                {
                    UnitInventory inv = unit.GetComponent<UnitInventory>();
                    if (inv != null && inv.CanAcceptItem(item))
                    {
                        int currentAmount = inv.GetItemCount(item);
                        if (currentAmount < minAmount)
                        {
                            minAmount = currentAmount;
                            bestTarget = inv;
                        }
                    }
                }
            }
            else
            {
                foreach (var unit in squad)
                {
                    UnitInventory inv = unit.GetComponent<UnitInventory>();
                    if (inv != null && inv.slots.Count < inv.maxSlots)
                    {
                        bestTarget = inv;
                        break;
                    }
                }
            }

            if (bestTarget != null)
            {
                bestTarget.AddResource(item, 1);
                remainingAmount--;
            }
            else
            {
                Debug.LogWarning($"<color=yellow>Инвентари отряда полны! На земле осталось: {remainingAmount} {item}</color>");
                break;
            }
        }

        if (remainingAmount == 0)
        {
            Debug.Log($"<color=green>Отряд успешно распределил {totalAmount} {item}!</color>");
        }
    }
}