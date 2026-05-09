using UnityEngine;
using System.Collections.Generic;

public class UnitInventory : MonoBehaviour
{
    public int capacity = 20;
    public ResourceType currentType;
    public int currentAmount = 0;

    public bool IsFull => currentAmount >= capacity;

    public void AddResource(ResourceType type, int amount)
    {
        currentType = type;
        currentAmount = Mathf.Min(currentAmount + amount, capacity);
        Debug.Log($"В инвентаре: {currentAmount} {currentType}");
    }
}