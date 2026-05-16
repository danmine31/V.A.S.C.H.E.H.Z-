using UnityEngine;
public class MainMenuCameraRotate : MonoBehaviour
{
    public float speed = 2.5f;
    void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}