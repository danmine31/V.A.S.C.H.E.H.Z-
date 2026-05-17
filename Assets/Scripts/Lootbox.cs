using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LootBoxType { Dropped, Chest }

public class LootBox : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public ItemType itemType;
        public int amount;
    }

    [Header("Настройки типа")]
    public LootBoxType boxType = LootBoxType.Dropped;
    public float lifeTime = 300f;

    [Header("Настройки вместимости ящика")]
    public int maxSlots = 15;
    public int maxStackSize = 67;

    [Header("Содержимое сундука")]
    public List<LootItem> boxContents = new List<LootItem>();

    void Start()
    {
        Collider col = GetComponent<Collider>();

        if (boxType == LootBoxType.Dropped)
        {
            Destroy(gameObject, lifeTime);
            if (col != null) col.isTrigger = true; 
        }
        else if (boxType == LootBoxType.Chest)
        {
            if (col != null) col.isTrigger = false;
        }
    }

    public void InteractWithBox(List<UnitController> squad)
    {
        StartCoroutine(WaitForSquadCoroutine(squad));
    }

    private IEnumerator WaitForSquadCoroutine(List<UnitController> squad)
    {
        if (squad == null || squad.Count == 0) yield break;
        UnitController leader = squad[0];

        while (leader != null && Vector3.Distance(leader.transform.position, transform.position) > 3f)
        {
            yield return null;
        }

        OpenLootUI(squad);
    }

    private void OpenLootUI(List<UnitController> squad)
    {
        LootUI ui = FindObjectOfType<LootUI>();
        if (ui != null)
        {
            ui.OpenLoot(this);
        }

        Debug.Log($"<color=yellow>[{gameObject.name}]: Открыто окно сундука! Ждем действий игрока...</color>");
    }

    public void AddItem(ItemType type, int amount)
    {
        foreach (var loot in boxContents)
        {
            if (loot.itemType == type && loot.amount < maxStackSize)
            {
                int spaceLeft = maxStackSize - loot.amount;
                int toAdd = Mathf.Min(amount, spaceLeft);
                loot.amount += toAdd;
                amount -= toAdd;
                if (amount <= 0) return;
            }
        }

        if (boxContents.Count < maxSlots && amount > 0)
        {
            boxContents.Add(new LootItem { itemType = type, amount = amount });
        }
    }

    public void CheckEmptyState()
    {
        if (boxContents.Count == 0)
        {
            if (boxType == LootBoxType.Dropped)
            {
                LootUI ui = FindObjectOfType<LootUI>();
                if (ui != null && ui.CurrentLootBox == this && ui.lootPanel.activeInHierarchy)
                {
                    return; 
                }

                Debug.Log("Трупный лут полностью собран и окно закрыто. Уничтожаем объект.");
                Destroy(gameObject);
            }
            else if (boxType == LootBoxType.Chest)
            {
                Debug.Log("Сюжетный сундук опустел, но остается стоять на карте.");
            }
        }
    }
}