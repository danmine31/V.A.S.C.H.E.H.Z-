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

    private void OnValidate()
    {
        if (boxContents != null)
        {
            if (boxContents.Count > maxSlots) boxContents.RemoveRange(maxSlots, boxContents.Count - maxSlots);
            foreach (var item in boxContents) if (item.amount > maxStackSize) item.amount = maxStackSize;
        }
    }

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

    public void InteractWithBox(List<EntityController> squad)
    {
        StartCoroutine(WaitForSquadCoroutine(squad));
    }

    private IEnumerator WaitForSquadCoroutine(List<EntityController> squad)
    {
        if (squad == null || squad.Count == 0) yield break;
        EntityController leader = squad[0];

        while (leader != null && Vector3.Distance(leader.transform.position, transform.position) > 3f)
        {
            yield return null;
        }

        OpenLootUI(squad);
    }

    private void OpenLootUI(List<EntityController> squad)
    {
        LootUI ui = FindAnyObjectByType<LootUI>();
        if (ui != null) ui.OpenLoot(this);
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
                LootUI ui = FindAnyObjectByType<LootUI>();
                if (ui != null && ui.CurrentLootBox == this && ui.lootPanel.activeInHierarchy) return; 
                Destroy(gameObject);
            }
            else if (boxType == LootBoxType.Chest)
            {
                Debug.Log("Сюжетный сундук опустел, но остается стоять на карте.");
            }
        }
    }
}