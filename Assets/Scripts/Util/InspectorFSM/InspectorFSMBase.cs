using System;
using UnityEngine;

public interface InspectorFSMBaseInterface<StateEnum>
    where StateEnum : Enum
{
    public StateEnum State { get; }

    void StartState();
    void EndState();
}

/// <summary>
///     Inspector를 활용한 FSM의 기본 클래스입니다. 해당 클래스를 상속받아 FSM 상태 클래스를 구현하시면 됩니다.
/// </summary>
/// <typeparam name="TStateEnum">Enum 형식의 FSM의 State입니다.</typeparam>
/// <typeparam name="TFSMSystem">InspectorFSMSystem을 상속한 클래스입니다.</typeparam>
public abstract class InspectorFSMBase<TStateEnum, TFSMSystem> : MonoBehaviour, InspectorFSMBaseInterface<TStateEnum>
    where TStateEnum : Enum
    where TFSMSystem : MonoBehaviour
{
    [SerializeField]
    private TStateEnum _state;

    /// <summary>
    ///     해당 FSM의 FSM System을 가져옵니다. (InspectorFSMSystem 상속)
    /// </summary>
    public TFSMSystem FsmSystem { get; private set; }

    /// <summary>
    ///     해당 코드에서 Awake() 함수 사용시 반드시 base.Awake(); 코드를 넣어주세요.
    /// </summary>
    protected virtual void Awake()
    {
        FsmSystem = GetComponent<TFSMSystem>();
    }

    /// <summary>
    ///     해당 코드에서 Reset() 함수 사용시 반드시 base.Reset(); 코드를 넣어주세요.
    /// </summary>
    protected void Reset()
    {
        Initialize();
    }

    /// <summary>
    ///     해당 FSM의 상태를 가져오거나 설정합니다.
    /// </summary>
    public TStateEnum State
    {
        get => _state;
        protected set => _state = value;
    }

    /// <summary>
    ///     해당 FSM이 실행이 된 경우 호출되는 함수입니다.
    /// </summary>
    public abstract void StartState();

    /// <summary>
    ///     해당 FSM이 끝나는 경우 호출되는 함수입니다.
    ///     다른 FSM의 StartState()가 호출되기 전 해당 함수가 먼저 호출됩니다.
    /// </summary>
    public abstract void EndState();

    /// <summary>
    ///     처음 InspectorFSMSystem 기반 FSM시스템을 처음 설정될 때 함께 호출되는 함수입니다.
    /// </summary>
    protected abstract void Initialize();

    /// <summary>
    ///     해당 FSM 시스템의 State를 바꾸는 함수입니다.
    ///     FSMBase 단계부터 구현해주시면 됩니다.
    ///     <code>[예시]
    ///     protected override void ChangeState([스테이트 Enum] state) {
    ///         System.ChangeState(state);
    ///     }</code>
    /// </summary>
    /// <param name="state">바꿀 State를 설정합니다.</param>
    protected abstract void ChangeState(TStateEnum state);
}
