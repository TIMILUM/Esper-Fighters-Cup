using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ColliderChecker : MonoBehaviour
{
    private UnityAction<ObjectBase> _onCollision = null;
    public event UnityAction<ObjectBase> OnCollision
    {
        add => _onCollision += value;
        remove => _onCollision -= value;
    }

    private void OnCollisionEnter(Collision other)
    {
        var target = other.gameObject.GetComponent<ObjectBase>();
        if (target == null) return;
        
        _onCollision?.Invoke(target);
    }

    private void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponent<ObjectBase>();
        if (target == null) return;
        
        _onCollision?.Invoke(target);
    }

}
