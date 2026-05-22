using UnityEngine;

public class MazeExit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        UnitController unit = other.GetComponentInParent<UnitController>();
        
        if (unit != null && unit.teamID == 1)
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.GameOverWin();
            }
        }
    }
}