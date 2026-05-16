using UnityEngine;
using System.Collections.Generic;

public class LootBox : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public ItemType itemType;
        public int amount;
    }

    [Header("Содержимое сундука")]
    public List<LootItem> boxContents = new List<LootItem>();

    [Header("Настройки")]
    public float interactionRadius = 2f;

    public void OpenBox(List<UnitController> squad)
    {
        if (boxContents.Count == 0)
        {
            Debug.Log($"[{gameObject.name}]: Сундук уже пуст!");
            return;
        }

        Debug.Log($"<color=cyan>[{gameObject.name}]: Отряд вскрывает сундук! Начинаем распределение...</color>");

        for (int i = boxContents.Count - 1; i >= 0; i--)
        {
            LootItem loot = boxContents[i];
            
            LootDistributor.DistributeAmongSquad(squad, loot.itemType, loot.amount);
            
            boxContents.RemoveAt(i);
        }

        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}