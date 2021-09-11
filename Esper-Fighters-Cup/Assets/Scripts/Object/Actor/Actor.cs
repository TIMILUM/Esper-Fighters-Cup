using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : ObjectBase
{

    [SerializeField, Tooltip("오브젝트를 직접 넣어주세요!")]
    protected ControllerManager _controllerManager = null;
    
    private void Awake()
    {
        _controllerManager?.SetActor(this);
    }

    protected override void OnHit(ObjectBase @from, ObjectBase to)
    {
    }
}
