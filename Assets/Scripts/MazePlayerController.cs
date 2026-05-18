using UnityEngine;

public class MazePlayerController : MonoBehaviour
{
    [Header("Слой земли для кликов")]
    public LayerMask groundLayer;

    private UnitController unitController; 

    void Start()
    {
        unitController = GetComponent<UnitController>();
        
        if (unitController == null)
        {
            Debug.LogError("На этом юните не найден скрипт UnitController! Движение невозможно.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            MoveToClickPosition();
        }
    }

    void MoveToClickPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, groundLayer))
        {
            Debug.Log("<color=green>УСПЕХ:</color> Луч попал в пол по координатам " + hit.point);
            
            if (unitController != null)
            {
                unitController.MoveTo(hit.point); 
                Debug.Log("<color=blue>ПРИКАЗ:</color> Команда MoveTo отправлена юниту.");
            }
        }
        else
        {
            Debug.LogWarning("<color=red>ОШИБКА:</color> Клик ушел в пустоту! Либо нет слоя Ground, либо на полу нет Коллайдера.");
        }
    }
}