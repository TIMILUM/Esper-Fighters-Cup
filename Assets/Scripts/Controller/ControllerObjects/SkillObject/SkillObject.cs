using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
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

    [SerializeField] private int _id;
    [SerializeField] private KeyCode _inputKey;

    private CSVData _commonCsvData;
    private int _commonCsvIndex;

    private CancellationTokenSource _stateCancellation;

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

    public KeyCode InputKey { get => _inputKey; set => _inputKey = value; }

    /// <summary>
    /// 현재 해당 스킬의 FSM입니다.
    /// </summary>
    public State CurrentState { get; private set; } = State.ReadyToUse;

    /// <summary>
    /// UI에 나타날 스킬 이름입니다.
    /// </summary>
    public string Name { get; set; }
    public int ID => _id;
    protected float Range { get; private set; }
    protected Vector2 Size { get; private set; }
    /// <summary>
    /// 해당 스킬이 사용되기 전 딜레이 밀리초 입니다.
    /// </summary>
    protected int FrontDelayMilliseconds { get; private set; }
    /// <summary>
    /// 해당 스킬이 사용되고 난 뒤 딜레이 밀리초 입니다.
    /// </summary>
    protected int EndDelayMilliseconds { get; private set; }
    protected float FrontDelayMoveSpeed { get; private set; }
    protected float EndDelayMoveSpeed { get; private set; }
    protected int StunGroggyDuration { get; private set; }
    protected int Damage { get; private set; }

    //Effects CSV Data
    protected Vector4 EffectData { get; private set; }
    protected Vector2 EffectSize { get; private set; }

    #region abstract

    /// <summary>
    /// 스킬 사용 전 캐스팅 단계입니다.
    /// </summary>
    protected abstract UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation);

    /// <summary>
    /// 캐스팅 후 스킬 사용하기까지 사이의 선 딜레이 진행 전 단계입니다.
    /// </summary>
    protected abstract void BeforeFrontDelay();

    /// <summary>
    /// 스킬이 본격적으로 실행되는 단계입니다.
    /// </summary>
    protected abstract UniTask OnUseAsync();

    /// <summary>
    /// 스킬 실행이 끝나고 스킬을 종료하기까지 사이의 후 딜레이 진행 전 단계입니다.
    /// </summary>
    protected abstract void BeforeEndDelay();

    /// <summary>
    /// 스킬이 끝나고 릴리즈되는 단계입니다.
    /// </summary>
    protected abstract void OnRelease();

    /// <summary>
    /// 스킬 사용이 취소되는 단계입니다.
    /// </summary>
    protected abstract void OnCancel();

    #endregion

    protected sealed override void OnInitialized()
    {
        OnInitializeSkill();
    }

    protected sealed override void OnRegistered(Action continueFunc)
    {
        Debug.Log($"[{ID}] OnRegistered");
        BuffController = Controller.ControllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
        AuthorPlayer = Author as APlayer;

        gameObject.SetActive(true);
        _stateCancellation = new CancellationTokenSource();
        RunAsync(_stateCancellation.Token, continueFunc).Forget();
    }

    protected sealed override void OnReleased()
    {
        Debug.Log($"[{ID}] OnReleased");
        ReleaseMoveSpeedBuffAll();
        gameObject.SetActive(false);

        _stateCancellation = null;
    }

    public sealed override void Release()
    {
        Debug.Log($"[{ID}] Release (Cancel)");
        _stateCancellation?.Cancel();
    }

    private async UniTaskVoid RunAsync(CancellationToken cancelltaion, Action afterFunc)
    {
        var isCanceled = await SkillReadyToUse(cancelltaion).ContinueWith(afterFunc).SuppressCancellationThrow();

        if (isCanceled)
        {
            Debug.Log($"[{ID}] Canceled");
            CurrentState = State.Canceled;
            ApplyMovementSpeed(State.Canceled);
            OnCancel();
            base.Release();
            afterFunc();
        }
    }

    private async UniTask SkillReadyToUse(CancellationToken cancelltaion)
    {
        Debug.Log($"[{ID}] ReadyToUse");
        CurrentState = State.ReadyToUse;
        await UniTask.Yield();
        var canMoveNextState = await OnReadyToUseAsync(cancelltaion);

        if (canMoveNextState)
        {
            await SkillFrontDelay(cancelltaion);
        }
        else
        {
            throw new OperationCanceledException(cancelltaion);
        }
    }

    private async UniTask SkillFrontDelay(CancellationToken cancelltaion)
    {
        Debug.Log($"[{ID}] FrontDelay {FrontDelayMilliseconds}");
        CurrentState = State.FrontDelay;
        ApplyMovementSpeed(State.FrontDelay);
        BeforeFrontDelay();
        await UniTask.Delay(FrontDelayMilliseconds, cancellationToken: cancelltaion);
        await SkillUse(cancelltaion);
    }

    private async UniTask SkillUse(CancellationToken cancelltaion)
    {
        Debug.Log($"[{ID}] Use");
        CurrentState = State.Use;
        ApplyMovementSpeed(State.Use);
        await OnUseAsync();
        await SkillEndDelay(cancelltaion);
    }

    private async UniTask SkillEndDelay(CancellationToken cancelltaion)
    {
        Debug.Log($"[{ID}] EndDelay {EndDelayMilliseconds}");
        CurrentState = State.EndDelay;
        ApplyMovementSpeed(State.EndDelay);
        BeforeEndDelay();
        await UniTask.Delay(EndDelayMilliseconds, cancellationToken: cancelltaion);
        SkillRelease();
    }

    private void SkillRelease()
    {
        Debug.Log($"[{ID}] Release");
        CurrentState = State.Release;
        ApplyMovementSpeed(State.Release);
        OnRelease();
        base.Release();
    }

    /// <summary>
    /// 스킬이 최초 생성되는 시점에 호출됩니다.<para/>
    /// 스킬이 실행되는 시점이 아닙니다! 스킬 실행 시점 콜백은 OnReadyToUse를 사용해주세요.
    /// </summary>
    protected virtual void OnInitializeSkill()
    {
        // CSV 데이터 적용
        _commonCsvData = CSVUtil.GetData("SkillDataTable");
        // csv 위치값
        _commonCsvData.Get<float>("Skill_ID", out var idList);
        _commonCsvIndex = idList.FindIndex(x => (int)x == _id);

        // Name = GetCSVData<string>("Name");
        Range = GetCSVData<float>("Range");
        Damage = (int)GetCSVData<float>("Damage");
        StunGroggyDuration = (int)GetCSVData<float>("Groggy_Duration");
        FrontDelayMilliseconds = (int)GetCSVData<float>("Pre_Delay_Duration");
        EndDelayMilliseconds = (int)GetCSVData<float>("After_Delay_Duration");
        FrontDelayMoveSpeed = GetCSVData<float>("Pre_Delay_MoveSpeed");
        EndDelayMoveSpeed = GetCSVData<float>("After_Delay_MoveSpeed");
        Size = new Vector2(GetCSVData<float>("ShapeData_1"), GetCSVData<float>("ShapeData_2"));

        // Effect Data
        EffectData = new Vector4(GetCSVData<float>("Skill_Effect_Data_1"), GetCSVData<float>("Skill_Effect_Data_2"), 0, 0);
        EffectSize = new Vector2(GetCSVData<float>("Effect_Size_1"), GetCSVData<float>("Effect_Size_2"));

        // TODO: 아래 코드 제거
        // 스턴 지속 시간
        var stunBuff = _buffOnCollision.Find(x => x.Type == BuffObject.Type.Stun);
        if (stunBuff != null)
        {
            stunBuff.Duration = GetCSVData<float>("Groggy_Duration") * 0.001f;
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

    private void ApplyMovementSpeed(State state)
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

        var value = state == State.FrontDelay ? FrontDelayMoveSpeed : EndDelayMoveSpeed;
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

        _generateMoveSpeedCoroutine = StartCoroutine(GenerateMoveSpeedBuff(value));
    }

    private void ReleaseMoveSpeedBuffAll()
    {
        foreach (var speedObject in _moveSpeedObjects)
        {
            BuffController.ReleaseBuff(speedObject);
        }

        _moveSpeedObjects.Clear();
    }

    private IEnumerator GenerateMoveSpeedBuff(float value)
    {
        var speedBuffs = BuffController.ActiveBuffs[BuffObject.Type.MoveSpeed];
        var leastMoveSpeedBuff = speedBuffs.Count > 0 ? speedBuffs.Last() : null;

        BuffController.GenerateBuff(new BuffObject.BuffStruct
        {
            Type = BuffObject.Type.MoveSpeed,
            Duration = 0f,
            AllowDuplicates = true,
            Damage = 0f,
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
