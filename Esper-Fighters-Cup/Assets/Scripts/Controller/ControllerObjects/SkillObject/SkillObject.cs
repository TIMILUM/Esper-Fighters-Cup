using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillObject : ControllerObject
{
    public enum State
    {
        ReadyToUse = 0,
        FrontDelay = 1,
        Use = 2,
        EndDelay = 3,
        Release = 4,
    }

    [SerializeField]
    private SkillObject.State _currentState = State.ReadyToUse;

    protected SkillObject.State CurrentState
    {
        get => _currentState;
        set => SetState(_currentState);
    }

    protected BuffController _buffController = null;
    protected Coroutine _currentCoroutine = null;

    public override void Register(ControllerBase controller)
    {
        base.Register(controller);
        _buffController =
            controller.ControllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
    }

    protected abstract IEnumerator OnReadyToUse();
    protected abstract IEnumerator OnFrontDelay();
    protected abstract IEnumerator OnUse();
    protected abstract IEnumerator OnEndDelay();
    protected abstract IEnumerator OnRelease();

    public void SetNextState()
    {
        if (_currentState >= State.Release) return;
        _currentState += 1;
        SetState(_currentState);
    }

    public void SetState(SkillObject.State state)
    {
        if (state < 0 || state > State.Release) return;
        _currentState = state;

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        var currentEnumerator = GetStateFunction();
        _currentCoroutine = StartCoroutine(currentEnumerator);
    }

    private IEnumerator GetStateFunction()
    {
        switch (_currentState)
        {
            case State.ReadyToUse:
                return OnReadyToUse();
            case State.FrontDelay:
                return OnFrontDelay();
            case State.Use:
                return OnUse();
            case State.EndDelay:
                return OnEndDelay();
            case State.Release:
                return OnRelease();
        }
        return null;
    }
}
