using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Экономика")]
    public int influencePoints = 0;
    public static event Action OnInfluenceChanged;

    [System.Serializable]
    public class PlayerSettings
    {
        public int ownerID;
        public int teamID;
        public string playerName;
        public Color playerColor;
    }

    [Header("Настройки участников матча")]
    public List<PlayerSettings> players = new List<PlayerSettings>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddInfluence(int amount)
    {
        influencePoints += amount;
        OnInfluenceChanged?.Invoke();
        Debug.Log($"<color=yellow>+ {amount} Влияния! Всего: {influencePoints}</color>");
    }

    public bool SpendInfluence(int amount)
    {
        if (influencePoints >= amount)
        {
            influencePoints -= amount;
            OnInfluenceChanged?.Invoke();
            return true;
        }
        Debug.LogWarning("Не хватает Очков Влияния!");
        return false;
    }

    public Color GetPlayerColor(int ownerID)
    {
        if (players == null || players.Count == 0) return Color.white;

        foreach (var player in players)
        {
            if (player.ownerID == ownerID)
            {
                return player.playerColor;
            }
        }
        return Color.white; 
    }

    public int GetPlayerTeam(int ownerID)
    {
        if (players == null || players.Count == 0) return ownerID;

        foreach (var player in players)
        {
            if (player.ownerID == ownerID)
            {
                return player.teamID;
            }
        }
        return ownerID; 
    }
}