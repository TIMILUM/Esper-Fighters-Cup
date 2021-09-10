using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    
    public enum ControllerType
    {
        None,
        MovementController,
        SkillController,
        BuffController,
    }

    private readonly Dictionary<ControllerType, ControllerBase> _controllers =
        new Dictionary<ControllerType, ControllerBase>();
    private Actor _actor = null;

    private void Start()
    {
        RegisterControllers();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public T GetController<T>(ControllerType type) where T : ControllerBase
    {
        if (!_controllers.ContainsKey(type)) return null;

        return (T)_controllers[type];
    }

    private void RegisterControllers()
    {
        var controllers = GetComponents<ControllerBase>();

        foreach (var controller in controllers)
        {
            controller.Register(this);
        }
    }

    public void RegisterController(ControllerType type, ControllerBase controller)
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
