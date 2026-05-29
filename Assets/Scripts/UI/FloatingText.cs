using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float destroyTime = 0.6f;
    public float floatSpeed = 4f;
    private TMP_Text textMesh;
    private float timer = 0f;
    private Vector3 startScale;

    public void Setup(float amount, bool isCrit, bool isHeal)
    {
        textMesh = GetComponent<TMP_Text>();
        if (textMesh == null) 
        {
            Debug.LogError("На префабе FloatingText нет компонента Text!");
            return;
        }

        Vector3 jitter = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        transform.position += jitter;

        textMesh.text = Mathf.Round(amount).ToString();

        if (isHeal) { textMesh.text = "+" + textMesh.text; textMesh.color = Color.green; }
        else if (isCrit) { textMesh.text = textMesh.text + "!"; textMesh.color = Color.red; }
        else { textMesh.color = Color.yellow; }

        startScale = transform.localScale;
        transform.localScale = Vector3.zero;

        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / destroyTime;

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        transform.LookAt(transform.position + Camera.main.transform.forward);

        float scaleMultiplier = Mathf.Sin(progress * Mathf.PI); 
        transform.localScale = startScale * (scaleMultiplier * 1.5f);

        if (textMesh != null)
        {
            Color c = textMesh.color;
            c.a = 1f - progress;
            textMesh.color = c;
        }
    }
}