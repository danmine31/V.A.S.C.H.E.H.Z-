using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float destroyTime = 0.8f;
    public float floatSpeed = 4f;
    private TMP_Text textMesh;

    public void Setup(float amount, bool isCrit, bool isHeal)
    {
        textMesh = GetComponent<TMP_Text>();
        if (textMesh == null) 
        {
            Debug.LogError("На префабе FloatingText нет компонента Text!");
            return;
        }

        textMesh.text = Mathf.Round(amount).ToString();

        if (isHeal)
        {
            textMesh.text = "+" + textMesh.text;
            textMesh.color = Color.green;
        }
        else if (isCrit)
        {
            textMesh.text = textMesh.text + "!";
            textMesh.color = Color.red;
            textMesh.fontSize += 2;
        }
        else
        {
            textMesh.color = Color.yellow;
        }

        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}