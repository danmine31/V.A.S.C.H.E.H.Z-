using UnityEngine;
public class MenuCameraRotate : MonoBehaviour
{
    public float speed = 2f;
    void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}