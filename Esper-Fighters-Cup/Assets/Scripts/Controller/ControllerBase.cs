using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;

public class ControllerBase : MonoBehaviourPunCallbacks
{
    protected ControllerManager _controllerManager = null;
    public ControllerManager ControllerManager => _controllerManager;

    [SerializeField]
    private ControllerManager.Type _type = ControllerManager.Type.None;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
    }

    public void Register(ControllerManager controllerManager)
    {
        _controllerManager = controllerManager;
        _controllerManager.RegisterController(_type, this);
    }

    protected virtual void SetControllerType(ControllerManager.Type type)
    {
        _type = type;
    }
}
