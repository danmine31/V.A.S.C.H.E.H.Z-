using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Transform target;
    public Vector3 offset = new Vector3(0, 2.5f, 0);
    public Canvas canvas;

    void Start()
    {
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
        }
        
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Canvas mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas != null)
            {
                transform.SetParent(mainCanvas.transform, false);
            }
        }
    }

    void Update()
    {
        if (target == null || target.gameObject == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        if (slider != null && maxValue > 0)
        {
            float healthPercent = currentValue / maxValue;
            slider.value = Mathf.Clamp01(healthPercent);
        }
    }
}
