using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ColliderChecker : MonoBehaviour
{
    private UnityAction<ObjectBase> _onCollision;
    private UnityAction<GameObject> _onCollisionAll;
    private ObjectBase _currentObjectBase = null;

    private void Awake()
    {
        _currentObjectBase = GetComponentInParent<ObjectBase>();
    }

    private void OnCollisionEnter(Collision other)
    {
        _onCollisionAll?.Invoke(other.gameObject);
        var target = FindObjectBase(other.gameObject);
        if (target == null)
        {
            return;
        }

        _onCollision?.Invoke(target);
    }

    private void OnTriggerEnter(Collider other)
    {
        _onCollisionAll?.Invoke(other.gameObject);
        var target = FindObjectBase(other.gameObject);
        if (target == null)
        {
            return;
        }

        _onCollision?.Invoke(target);
    }

    private ObjectBase FindObjectBase(GameObject gameObj)
    {
        var target = gameObj.GetComponent<ObjectBase>();
        if (target != null)
        {
            return target;
        }

        var checker = gameObj.GetComponent<ColliderChecker>();
        if (checker == null)
        {
            return null;
        }

        target = checker._currentObjectBase;

        return target;
    }

    public event UnityAction<ObjectBase> OnCollision
    {
        add => _onCollision += value;
        remove => _onCollision -= value;
    }

    public event UnityAction<GameObject> OnCollisionAll
    {
        add => _onCollisionAll += value;
        remove => _onCollisionAll -= value;
    }
}
