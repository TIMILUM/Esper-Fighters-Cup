using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class SkillObject : ControllerObject<SkillController>
{
    /// <summary>
    /// 스킬 FSM의 상태변화가 정리된 enum입니다.
    /// </summary>
    public enum State
    {
        ReadyToUse = 0,
        FrontDelay,
        Use,
        EndDelay,
        Canceled,
        Release
    }

    [SerializeField]
    private State _currentState = State.ReadyToUse;

    [SerializeField]
    private int _id;

    private CSVData _commonCsvData;
    private int _commonCsvIndex;

    /// <summary>
    /// 이 스킬에서 현재 실행 중인 코루틴입니다. (코루틴의 수정은 지정된 함수로 진행하는걸 권장합니다.)
    /// </summary>
    protected Coroutine _currentCoroutine;

    private float _frontDelayMoveSpeed;
    private float _endDelayMoveSpeed;

    // 움직임 버프 관련하여 안전하게 생성하는 코루틴 함수입니다.
    private Coroutine _generateMoveSpeedCoroutine;

    /// <summary>
    /// 스킬 캐스팅 중 움직임 관련하여 주어진 버프를 따로 모아 안전하게 해지하고자 합니다.
    /// </summary>
    private readonly List<MoveSpeedObject> _moveSpeedObjects = new List<MoveSpeedObject>();

    /// <summary>
    /// 플레이어
    /// </summary>
    protected APlayer AuthorPlayer { get; private set; }

    /// <summary>
    /// 플레이어의 버프 컨트롤러입니다.
    /// </summary>
    protected BuffController BuffController { get; private set; }

    /// <summary>
    /// 현재 해당 스킬의 FSM입니다.
    /// </summary>
    public State CurrentState => _currentState;

    /// <summary>
    /// 해당 스킬을 중복으로 호출(생성)이 가능한지에 대한 여부입니다.
    /// </summary>
    public bool AllowDuplicates { get; protected set; } = false;

    /// <summary>
    /// UI에 나타날 스킬 이름입니다.
    /// </summary>
    public string Name { get; set; }

    public int ID => _id;

    /// <summary>
    /// 해당 스킬이 사용되기 전 딜레이 밀리초 입니다. <para/>
    /// @Todo 나중에 스킬 작업 내용 모두 머지하면 SkillObject에서 데이터 적용하도록 수정이 필요함.
    /// </summary>
    protected float FrontDelayMilliseconds { get; private set; }

    /// <summary>
    /// 해당 스킬이 사용되고 난 뒤 딜레이 밀리초 입니다.
    /// </summary>
    protected float EndDelayMilliseconds { get; private set; }

    protected sealed override void OnRegistered()
    {
        BuffController = Controller.ControllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
        AuthorPlayer = Author as APlayer;

        LoadSkillData();
        if (Author.photonView.IsMine)
        {
            SyncState(State.ReadyToUse);
        }
    }

    protected sealed override void OnReleased()
    {
        if (Author.photonView.IsMine)
        {
            ReleaseMoveSpeedBuffAll();
        }
    }

    /// <summary>
    /// 스킬 사용 전 캐스팅 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnReadyToUse();

    /// <summary>
    /// 캐스팅 후 스킬 사용하기까지 사이의 선 딜레이 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnFrontDelay();

    /// <summary>
    /// 스킬이 본격적으로 실행되는 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnUse();

    /// <summary>
    /// 스킬 실행이 끝나고 스킬을 종료하기까지 사이의 후 딜레이 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnEndDelay();

    /// <summary>
    /// 스킬 사용이 취소되는 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnCanceled();

    /// <summary>
    /// 스킬이 끝나고 릴리즈되는 단계입니다.
    /// </summary>
    protected abstract IEnumerator OnRelease();

    /// <summary>
    /// 다음 단계의 스킬 FSM으로 이동합니다. 스킬의 PhotonView Controller 본인이 아니면 작동하지 않습니다.
    /// </summary>
    public void SetNextState()
    {
        if (Author.photonView.IsMine && _currentState < State.Release)
        {
            _currentState += 1;
            SyncState(_currentState);
        }
    }

    /// <summary>
    /// 스킬의 State를 새로운 State로 동기화합니다. 스킬의 PhotonView Controller 본인이 아니면 작동하지 않습니다. <para/>
    /// 인스펙터 및 애니메이터에 사용하는 용도로 해당 함수를 삭제하지 마세요!
    /// </summary>
    /// <param name="state">이동할 스킬 FSM의 상태입니다.</param>
    public void SyncState(State state)
    {
        if (Author.photonView.IsMine)
        {
            Controller.ChangeState(ID, state);
        }
    }

    /// <summary>
    /// 스킬의 FSM을 변경해줍니다. <para/>
    /// 로컬에서만 적용되기 때문에 모든 클라이언트에서의 State 동기화는 <see cref="SyncState(State)"/>를 사용해주세요.
    /// </summary>
    /// <param name="state"></param>
    public void SetStateToLocal(State state)
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
            _ => null
        };
    }

    /// <summary>
    /// 스킬이 최초 생성되는 시점에 CSV 로딩을 위해 호출됩니다.<para/>
    /// 스킬이 실행되는 시점이 아닙니다! 스킬 실행 시점 콜백은 OnReadyToUse를 사용해주세요.
    /// </summary>
    protected virtual void LoadSkillData()
    {
        // CSV 데이터 적용
        _commonCsvData = CSVUtil.GetData("SkillDataTable");
        // csv 위치값
        _commonCsvData.Get<float>("Skill_ID", out var idList);
        _commonCsvIndex = idList.FindIndex(x => (int)x == _id);

        // 이름
        Name = GetCSVData<string>("Name");
        // 선 딜레이
        FrontDelayMilliseconds = GetCSVData<float>("Pre_Delay_Duration");
        // 후 딜레이
        EndDelayMilliseconds = GetCSVData<float>("After_Delay_Duration");
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
        if (!Author.photonView.IsMine)
        {
            return;
        }

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

        IngameDelayCursorObject.SetActiveCursor(true, value / 1000.0f);
        if (_generateMoveSpeedCoroutine != null)
        {
            StopCoroutine(_generateMoveSpeedCoroutine);
            _generateMoveSpeedCoroutine = null;
        }

        _generateMoveSpeedCoroutine = StartCoroutine(GenerateMoveSpeedBuffCoroutine(value));
    }

    private void ReleaseMoveSpeedBuffAll()
    {
        foreach (var speedObject in _moveSpeedObjects)
        {
            BuffController.ReleaseBuff(speedObject);
        }

        _moveSpeedObjects.Clear();
    }

    private IEnumerator GenerateMoveSpeedBuffCoroutine(float value)
    {
        var speedBuffs = BuffController.ActiveBuffs[BuffObject.Type.MoveSpeed];
        var leastMoveSpeedBuff = speedBuffs.Count > 0 ? speedBuffs.Last() : null;

        BuffController.GenerateBuff(new BuffObject.BuffStruct
        {
            Type = BuffObject.Type.MoveSpeed,
            Duration = 0,
            AllowDuplicates = true,
            Damage = 0,
            IsOnlyOnce = false,
            ValueFloat = new[] { value }
        });

        IReadOnlyList<BuffObject> moveSpeedBuffList = null;

        // 만들기 전 최신의 움직임 버프 ID값과 비교하여 실제로 구현이 되었는지 확인합니다.
        yield return new WaitUntil(() =>
        {
            var leastBuffId = leastMoveSpeedBuff != null ? leastMoveSpeedBuff.BuffId : null;

            moveSpeedBuffList = BuffController.ActiveBuffs[BuffObject.Type.MoveSpeed];

            // TODO: 테스트 필요
            var currentBuffId = moveSpeedBuffList.Count > 0 ? moveSpeedBuffList.Last().BuffId : null;
            return currentBuffId != null && !currentBuffId.Equals(leastBuffId);
        });

        if (moveSpeedBuffList != null)
        {
            _moveSpeedObjects.Add(moveSpeedBuffList[moveSpeedBuffList.Count - 1] as MoveSpeedObject);
        }
    }
}
