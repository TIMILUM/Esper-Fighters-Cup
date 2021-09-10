using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;

public class ControllerBase : MonoBehaviourPunCallbacks
{
    [SerializeField, ReadOnly]
    protected readonly List<ControllerObject> _controllerObjects = new List<ControllerObject>();

    protected ControllerManager _controllerManager = null;
    private ControllerManager.ControllerType _type = ControllerManager.ControllerType.None;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    public void Register(ControllerManager controllerManager)
    {
        _controllerManager = controllerManager;
        _controllerManager.RegisterController(_type, this);
    }

    protected virtual void SetControllerType(ControllerManager.ControllerType type)
    {
        _type = type;
    }
}
