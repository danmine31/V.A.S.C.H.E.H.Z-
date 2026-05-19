using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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