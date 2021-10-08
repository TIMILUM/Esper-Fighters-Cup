using System;
using System.Collections;
using UnityEngine;

public abstract class SkillObject : ControllerObject
{
    /// <summary>
    ///     스킬 FSM의 상태변화가 정리된 enum입니다.
    /// </summary>
    public enum State
    {
        ReadyToUse = 0,
        FrontDelay = 1,
        Use = 2,
        EndDelay = 3,
        Canceled = 4,
        Release = 5
    }

    [SerializeField]
    private State _currentState = State.ReadyToUse;

    private short _physicsCount = -1;

    /// <summary>
    ///     플레이어의 버프 컨트롤러입니다.
    /// </summary>
    protected BuffController _buffController;

    /// <summary>
    ///     이 스킬에서 현재 실행 중인 코루틴입니다. (코루틴의 수정은 지정된 함수로 진행하는걸 권장합니다.)
    /// </summary>
    protected Coroutine _currentCoroutine;

    /// <summary>
    ///     현재 해당 스킬의 FSM입니다.
    /// </summary>
    protected State CurrentState
    {
        get => _currentState;
        set => SetState(_currentState);
    }

    /// <summary>
    ///     해당 스킬을 중복으로 호출(생성)이 가능한지에 대한 여부입니다.
    /// </summary>
    public bool AllowDuplicates { get; protected set; } = false;

    /// <summary>
    ///     UI에 나타날 스킬 이름입니다.
    /// </summary>
    public string Name { get; set; }

    protected virtual void Start()
    {
        SetState(State.ReadyToUse);
    }

    protected void FixedUpdate()
    {
        if (_physicsCount > 0)
        {
            --_physicsCount;
        }
    }

    public override void Register(ControllerBase controller)
    {
        base.Register(controller);
        _buffController =
            controller.ControllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
    }

    /// <summary>
    ///     스킬 사용 전 캐스팅 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnReadyToUse();

    /// <summary>
    ///     캐스팅 후 스킬 사용하기까지 사이의 선 딜레이 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnFrontDelay();

    /// <summary>
    ///     스킬이 본격적으로 실행되는 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnUse();

    /// <summary>
    ///     스킬 실행이 끝나고 스킬을 종료하기까지 사이의 후 딜레이 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnEndDelay();

    /// <summary>
    ///     스킬 사용이 취소되는 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnCanceled();

    /// <summary>
    ///     스킬이 끝나고 릴리즈되는 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnRelease();

    /// <summary>
    ///     다음 단계의 스킬 FSM으로 이동합니다.
    /// </summary>
    public void SetNextState()
    {
        if (_currentState >= State.Release)
        {
            return;
        }

        _currentState += 1;
        SetState(_currentState);
    }

    /// <summary>
    ///     특정 단계의 스킬 FSM으로 이동합니다.
    ///     인스펙터 및 애니메이터에 사용하는 용도로 해당 함수를 삭제하지 마세요!
    /// </summary>
    /// <param name="state">이동할 스킬 FSM의 상태입니다.</param>
    public void SetState(State state)
    {
        if (state < 0 || state > State.Release)
        {
            return;
        }

        _currentState = state;

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }

        var currentEnumerator = GetStateFunction();
        _currentCoroutine = StartCoroutine(currentEnumerator);
    }

    /// <summary>
    /// WaitPhysicsUpdate()를 초기화시켜주는 함수입니다.
    /// WaitPhysicsUpdate() 함수를 재활용할 때 해당 함수를 실행시켜야합니다.
    /// </summary>
    protected void ResetPhysicsUpdateCount()
    {
        _physicsCount = -1;
    }
    
    /// <summary>
    /// 코루틴의 yield return 을 통해 물리 연산이 몇 번 실행되었는지 알 수 있는 함수입니다.
    /// 해당 함수를 재활용하기 위해선 ResetPhysicsUpdateCount()를 한번 실행해야합니다.
    /// </summary>
    /// <param name="waitCount">해당 정수만큼 연산 횟수를 기다립니다.</param>
    /// <returns></returns>
    protected bool WaitPhysicsUpdate(short waitCount = 1)
    {
        if (_physicsCount < 0)
        {
            _physicsCount = waitCount;
        }

        return _physicsCount <= 0;
    }

    private IEnumerator GetStateFunction()
    {
        return _currentState switch
        {
            State.ReadyToUse => OnReadyToUse(),
            State.FrontDelay => OnFrontDelay(),
            State.Use => OnUse(),
            State.EndDelay => OnEndDelay(),
            State.Canceled => OnCanceled(),
            State.Release => OnRelease(),
            _ => null,
        };
    }
}
