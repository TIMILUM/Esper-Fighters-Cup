using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerObject : ObjectBase
{
    protected ControllerBase _controller = null;

    public virtual void Register(ControllerBase controller)
    {
        _controller = controller;
    }

    protected T ControllerCast<T>() where T : ControllerBase
    {
        return (T)_controller;
    }
}
