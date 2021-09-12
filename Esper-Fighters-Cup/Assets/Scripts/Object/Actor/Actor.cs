using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : ObjectBase
{

    [SerializeField, Tooltip("오브젝트를 직접 넣어주세요!")]
    protected ControllerManager _controllerManager = null;

    protected BuffController _buffController = null;
    
    protected virtual void Awake()
    {
        _controllerManager?.SetActor(this);
    }

    protected virtual void Start()
    {
        _buffController = _controllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
    }

    protected override void OnHit(ObjectBase @from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
        if (_buffController == null) return;
        foreach (var buffStruct in appendBuff)
        {
            _buffController.GenerateBuff(buffStruct);
        }
    }
}
