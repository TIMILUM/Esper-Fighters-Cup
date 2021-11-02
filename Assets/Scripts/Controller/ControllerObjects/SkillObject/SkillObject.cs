using System;
using System.Collections;
using System.Collections.Generic;
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

    /// <summary>
    ///  플레이어
    /// </summary>
    protected APlayer _player;

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
    public State CurrentState
    {
        get => _currentState;
        protected set => SetState(_currentState);
    }

    /// <summary>
    ///     해당 스킬을 중복으로 호출(생성)이 가능한지에 대한 여부입니다.
    /// </summary>
    public bool AllowDuplicates { get; protected set; } = false;

    /// <summary>
    ///     UI에 나타날 스킬 이름입니다.
    /// </summary>
    public string Name { get; set; }

    [SerializeField]
    private int _id = 0;
    public int ID => _id;

    private int _commonCsvIndex = 0;

    private float _frontDelayMilliseconds = 0;
    private float _endDelayMilliseconds = 0;
    private float _frontDelayMoveSpeed = 0;
    private float _endDelayMoveSpeed = 0;

    /// <summary>
    ///     해당 스킬이 사용되기 전 딜레이 밀리초 입니다.
    ///     @Todo 나중에 스킬 작업 내용 모두 머지하면 SkillObject에서 데이터 적용하도록 수정이 필요함.
    /// </summary>
    protected float FrontDelayMilliseconds
    {
        get => _frontDelayMilliseconds;
    }

    /// <summary>
    ///     해당 스킬이 사용되고 난 뒤 딜레이 밀리초 입니다.
    /// </summary>
    protected float EndDelayMilliseconds
    {
        get => _endDelayMilliseconds;
    }

    /// <summary>
    ///     스킬 캐스팅 중 움직임 관련하여 주어진 버프를 따로 모아 안전하게 해지하고자 합니다.
    /// </summary>
    private List<MoveSpeedObject> _moveSpeedObjects = new List<MoveSpeedObject>();

    private CSVData _commonCsvData = null;

    protected virtual void Start()
    {
        SetCSVData();
        SetState(State.ReadyToUse);
    }

    protected override void OnRegistered()
    {
        _buffController = Controller.ControllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
        _player = Author.GetComponent<APlayer>();
    }

    protected void FixedUpdate()
    {
        if (_physicsCount > 0)
        {
            --_physicsCount;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ReleaseMoveSpeedBuffAll();
        ControllerCast<SkillController>().ReleaseSkill(this);
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

    private void SetCSVData()
    {
        // CSV 데이터 적용
        _commonCsvData = CSVUtil.GetData("SkillDataTable");
        // csv 위치값
        _commonCsvData.Get<float>("Skill_ID", out var idList);
        _commonCsvIndex = idList.FindIndex(x => (int)x == _id);

        // 이름
        Name = GetCSVData<string>("Name");
        // 선 딜레이
        _frontDelayMilliseconds = GetCSVData<float>("Pre_Delay_Duration");
        // 후 딜레이
        _endDelayMilliseconds = GetCSVData<float>("After_Delay_Duration");
        // 선 딜레이 이동 속도
        _frontDelayMoveSpeed = GetCSVData<float>("Pre_Delay_MoveSpeed");
        // 후 딜레이 이동 속도
        _endDelayMoveSpeed = GetCSVData<float>("After_Delay_MoveSpeed");
        // 스턴 지속 시간
        var stunBuff = _buffOnCollision.Find(x => x.Type == BuffObject.Type.Stun);
        if (stunBuff != null)
        {
            stunBuff.Duration = GetCSVData<float>("Groggy_Duration");
        }

        // 데미지
        var decreaseHpBuff = _buffOnCollision.Find(x => x.Type == BuffObject.Type.DecreaseHp);
        if (decreaseHpBuff != null)
        {
            decreaseHpBuff.Damage = GetCSVData<float>("Damage");
        }
    }

    /// <summary>
    /// 해당 스킬의 CSV 데이터를 들고옵니다.
    /// 캐스팅 시 float, bool, string만 허용됩니다.
    /// </summary>
    /// <param name="key">가져올 데이터의 이름입니다.</param>
    /// <typeparam name="T">캐스팅 시 float, bool, string만 허용됩니다.</typeparam>
    protected T GetCSVData<T>(string key)
    {
        if (!_commonCsvData.Get<T>(key, out var valueList))
        {
            throw new Exception("Error to parse csv data.");
        }

        return valueList[_commonCsvIndex];
    }

    protected void ApplyMovementSpeed(State state)
    {
        // 나머지 상태인 경우 움직임 버프와 관련한 요소가 없으므로 버프요소 삭제
        if (state != State.FrontDelay && state != State.EndDelay)
        {
            ReleaseMoveSpeedBuffAll();
            return;
        }

        var value = state == State.FrontDelay ? _frontDelayMoveSpeed : _endDelayMoveSpeed;
        ReleaseMoveSpeedBuffAll();

        if (value <= 0)
        {
            return;
        }
            
        _buffController.GenerateBuff(new BuffObject.BuffStruct()
        {
            Type = BuffObject.Type.MoveSpeed,
            Duration = 0,
            AllowDuplicates = true,
            Damage = 0,
            IsOnlyOnce = false,
            ValueFloat = new [] { value },
        });
        var moveSpeedBuffList = _buffController.GetBuff(BuffObject.Type.MoveSpeed);
        if (moveSpeedBuffList != null)
        {
            _moveSpeedObjects.Add(moveSpeedBuffList[moveSpeedBuffList.Count - 1] as MoveSpeedObject);
        }
    }

    protected void ReleaseMoveSpeedBuffAll()
    {
        foreach (var speedObject in _moveSpeedObjects)
        {
            _buffController.ReleaseBuff(speedObject);
        }
        _moveSpeedObjects.Clear();
    }
}
