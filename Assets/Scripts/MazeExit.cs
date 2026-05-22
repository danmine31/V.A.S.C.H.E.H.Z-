using UnityEngine;

public class MazeExit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        UnitStats stats = other.GetComponentInParent<UnitStats>();
        
        if (stats != null && stats.teamID == 1)
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.GameOverWin();
            }
        }
    }
}