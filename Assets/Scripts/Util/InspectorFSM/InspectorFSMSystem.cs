using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InspectorFSMInterface<TStateEnum, TBaseClass>
    where TBaseClass : InspectorFSMBaseInterface<TStateEnum>
    where TStateEnum : System.Enum
{
    Dictionary<TStateEnum, TBaseClass> StatePool { get; }
    TStateEnum StartState { get; }
    TStateEnum CurrState { get; }

    void ChangeState(TStateEnum state);
}

/// <summary>
/// Inspector를 활용한 FSM System 클래스입니다. 해당 클래스를 상속받아 FSM System을 커스텀하시면 됩니다.
/// </summary>
/// <typeparam name="TStateEnum">Enum 형식의 FSM의 State입니다.</typeparam>
/// <typeparam name="TBaseClass">InspectorFSMBase를 상속한 클래스입니다.</typeparam>
public abstract class InspectorFSMSystem<TStateEnum,TBaseClass> : MonoBehaviour, InspectorFSMInterface<TStateEnum, TBaseClass>
    where TStateEnum : System.Enum
    where TBaseClass : InspectorFSMBaseInterface<TStateEnum>
{

    public Dictionary<TStateEnum, TBaseClass> StatePool { get; private set; }

    [SerializeField]
    private TStateEnum _startState;
    public TStateEnum StartState => _startState;

    private TStateEnum _currState;
    public TStateEnum CurrState => _currState;

    protected virtual void Awake()
    {
        StatePool = new Dictionary<TStateEnum, TBaseClass>();
        InitStatePool();
    }

    /// <summary>
    /// 해당 FSM 시스템의 State를 바꿉니다.
    /// </summary>
    /// <param name="state">바꿀 State를 설정합니다.</param>
    public void ChangeState(TStateEnum state)
    {
        if (!StatePool.TryGetValue(state, out var nextState))
        {
            Debug.LogError("Dont have Key in InspectorFSMSystem");
            return;
        }
        var nextStateObject = CastToMonoBehavior(nextState);

        if (StatePool.TryGetValue(_currState, out var currentState))
        {
            currentState.EndState();
        }

        foreach (var pair in StatePool)
        {
            CastToMonoBehavior(pair.Value).enabled = false;
        }

        _currState = state;
        nextStateObject.enabled = true;
        nextState.StartState();
    }

    protected virtual void InitStatePool()
    {
        var states = GetComponents<TBaseClass>();
        foreach (var state in states)
        {
            // 모든 스테이트를 비활성화 시킵니다.
            var stateObject = CastToMonoBehavior(state);
            stateObject.enabled = false;

            if (StatePool.ContainsKey(state.State))
            {
                continue;
            }

            StatePool[state.State] = state;
        }
        // 첫 스테이트로 전환해줍니다.
        ChangeState(StartState);
    }

    private MonoBehaviour CastToMonoBehavior(TBaseClass state)
    {
        return state as MonoBehaviour;
    }
}
