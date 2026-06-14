using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    public abstract void MoveTo(Vector3 point);
    public abstract void SetTarget(Health enemy);
    public abstract void SetResourceTarget(ResourceSource resource);
}