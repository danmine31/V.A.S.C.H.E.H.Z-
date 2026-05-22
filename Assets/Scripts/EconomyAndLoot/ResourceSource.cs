using UnityEngine;

public class ResourceSource : MonoBehaviour
{
    [Header("Resource Settings")]
    public ItemType type;
    public int amountRemaining = 100;
    public float gatherTime = 1.5f;
    public int Gather(int amount)
    {
        if (amountRemaining <= 0)
            return 0;

        int gathered = Mathf.Min(amount, amountRemaining);
        amountRemaining -= gathered;

        if (amountRemaining <= 0)
        {
            Debug.Log($"{type} закончился!");
            Destroy(gameObject);
        }

        return gathered;
    }
}