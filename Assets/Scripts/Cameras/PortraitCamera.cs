using UnityEngine;

public class PortraitCamera : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null) 
        {
            cam.fieldOfView = 25f;
        }
    }

    void LateUpdate()
    {
        if (SelectionController.Instance == null) return;

        EntityController selected = SelectionController.Instance.GetMainSelectedController();
        EntityStats inspected = SelectionController.Instance.GetInspectedEntity();

        Transform target = null;
        if (selected != null) target = selected.transform;
        else if (inspected != null) target = inspected.transform;

        if (target != null)
        {
            transform.position = target.position + target.forward * 2.5f + target.right * 1f + Vector3.up * 1.5f;
            transform.LookAt(target.position + Vector3.up * 1.5f);
        }
    }
}