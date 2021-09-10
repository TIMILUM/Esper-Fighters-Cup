using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerObject : ObjectBase
{
    protected ControllerBase _controller = null;
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    public void Register(ControllerBase controller)
    {
        _controller = controller;
    }

    protected T GetController<T>() where T : ControllerBase
    {
        return (T)_controller;
    }
}
