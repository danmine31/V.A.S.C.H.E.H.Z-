using UnityEngine;
using System.Collections.Generic;

public class RadiusVisualizer : MonoBehaviour
{
    private List<LineRenderer> activeCircles = new List<LineRenderer>();

    public void AddRadius(float radius, Color color, string radiusName)
    {
        LineRenderer lr = CreateCircleRenderer(radiusName, color, radius);
        activeCircles.Add(lr);

        lr.enabled = SelectionController.isRadiusesVisible;
    }

    public void ToggleRadiuses(bool show)
    {
        foreach (var circle in activeCircles)
        {
            if (circle != null) circle.enabled = show;
        }
    }

    private LineRenderer CreateCircleRenderer(string name, Color color, float radius)
    {
        GameObject circleObj = new GameObject(name);
        circleObj.transform.SetParent(this.transform);
        circleObj.transform.localPosition = new Vector3(0, 0.05f, 0);

        LineRenderer lr = circleObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.startWidth = 0.08f;
        lr.endWidth = 0.08f;
        
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;

        int segments = 40;
        lr.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, 0, z));
        }

        return lr;
    }
}