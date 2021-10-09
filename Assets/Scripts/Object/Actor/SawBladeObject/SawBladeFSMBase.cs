using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SawBladeObject))]
public abstract class SawBladeFSMBase : InspectorFSMBase<SawBladeFSMSystem.StateEnum, SawBladeFSMSystem>
{

    protected SawBladeObject _sawBladeObject = null;

    protected override void Awake()
    {
        _sawBladeObject = GetComponent<SawBladeObject>();
        base.Awake();
    }

    protected virtual void Start()
    {
        if (!FsmSystem.BuffControllerObject.photonView.IsMine)
        {
            enabled = false;
        }
    }

    protected override void ChangeState(SawBladeFSMSystem.StateEnum state)
    {
        FsmSystem.ChangeState(state);
    }
}
