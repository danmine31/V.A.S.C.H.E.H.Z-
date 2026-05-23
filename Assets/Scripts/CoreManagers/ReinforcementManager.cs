using UnityEngine;
using UnityEngine.UI;

public class ReinforcementManager : MonoBehaviour
{
    [Header("Настройки призыва")]
    public GameObject soldierPrefab;
    public Transform spawnPoint;
    public int cost = 10;
    public float cooldown = 20f;
    
    [Header("UI Кнопка")]
    public Button buyButton;

    private float nextSpawnTime = -1f;

    void Update()
    {
        if (buyButton != null && GameManager.Instance != null)
        {
            bool canAfford = GameManager.Instance.influencePoints >= cost;
            bool isCooldownReady = Time.time >= nextSpawnTime;
            
            buyButton.interactable = canAfford && isCooldownReady;

            if (Input.GetKeyDown(KeyCode.B) && buyButton.interactable)
            {
                BuySoldier();
            }
        }
    }

    public void BuySoldier()
    {
        if (Time.time >= nextSpawnTime && GameManager.Instance.SpendInfluence(cost))
        {
            Instantiate(soldierPrefab, spawnPoint.position, Quaternion.identity);
            nextSpawnTime = Time.time + cooldown;
            Debug.Log($"<color=green>Подкрепление вызвано! КД {cooldown} сек.</color>");
        }
    }
}