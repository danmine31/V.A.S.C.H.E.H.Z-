using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Slider actionBar;
    public Image fillImage;
    public TextMeshProUGUI levelText;
    public Transform target;
    public Vector3 offset = new Vector3(0, 2.5f, 0);

    void Update()
    {
        if (target == null) return;

        transform.position = target.position + offset;
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }

    public void SetColor(Color c) 
    {
        Color solidColor = new Color(c.r, c.g, c.b, 0.95f);

        if (fillImage != null) 
        {
            fillImage.color = solidColor;
        }

        if (levelText != null)
        {
            levelText.color = Color.white; 
        }
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = currentValue;
    }

    public void UpdateActionBar(float progress)
    {
        if (actionBar != null)
        {
            actionBar.gameObject.SetActive(progress > 0f && progress < 1f);
            actionBar.value = progress;
        }
    }

    public void UpdateLevelText(int level)
    {
        if (levelText != null) levelText.text = level.ToString();
    }
}