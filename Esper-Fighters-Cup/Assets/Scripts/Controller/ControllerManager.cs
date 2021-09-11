using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    
    public enum Type
    {
        None,
        MovementController,
        SkillController,
        BuffController,
    }

    private readonly Dictionary<Type, ControllerBase> _controllers =
        new Dictionary<Type, ControllerBase>();
    private Actor _actor = null;

    protected void Awake()
    {
        RegisterControllers();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public T GetController<T>(Type type) where T : ControllerBase
    {
        if (!_controllers.ContainsKey(type)) return null;

        return (T)_controllers[type];
    }

    private void RegisterControllers()
    {
        var controllers = GetComponentsInChildren<ControllerBase>();

        foreach (var controller in controllers)
        {
            controller.Register(this);
        }
    }

    public void RegisterController(Type type, ControllerBase controller)
    {
        if (_controllers.ContainsKey(type)) return;
        _controllers.Add(type, controller);
    }

    public void SetActor(Actor actor)
    {
        _actor = actor;
    }

    public Actor GetActor()
    {
        return _actor;
    }
}
