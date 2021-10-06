using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public enum Type
    {
        None,
        MovementController,
        SkillController,
        BuffController
    }

    private readonly Dictionary<Type, ControllerBase> _controllers =
        new Dictionary<Type, ControllerBase>();

    private Actor _actor;

    protected void Awake()
    {
        RegisterControllers();
    }

    public T GetController<T>(Type type) where T : ControllerBase
    {
        if (!_controllers.TryGetValue(type, out var controller))
        {
            return null;
        }

        return (T)controller;
    }

    public bool TryGetController<T>(Type type, out T controller) where T : ControllerBase
    {
        controller = GetController<T>(type);
        return controller != null;
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
        if (_controllers.ContainsKey(type))
        {
            return;
        }

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

    public void OnPlayerHitEnter(GameObject other)
    {
        foreach (var controllerPair in _controllers)
        {
            var controller = controllerPair.Value;
            controller.OnPlayerHitEnter(other);
        }
    }
}
