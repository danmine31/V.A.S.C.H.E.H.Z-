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

        OpenBox(squad);
    }

    public void OpenBox(List<UnitController> squad)
    {
        if (boxContents.Count == 0) return;

        Debug.Log($"<color=cyan>[{gameObject.name}]: Отряд вскрывает лут! Начинаем распределение...</color>");

        for (int i = boxContents.Count - 1; i >= 0; i--)
        {
            LootItem loot = boxContents[i];
            LootDistributor.DistributeAmongSquad(squad, loot.itemType, loot.amount);
            boxContents.RemoveAt(i);
        }

        InventoryUI ui = FindFirstObjectByType<InventoryUI>();
        if (ui != null && ui.inventoryPanel.activeInHierarchy)
        {
            ui.UpdateUI();
        }

        Destroy(gameObject, 0.5f);
    }
}