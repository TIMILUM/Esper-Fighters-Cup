using System;
using UnityEngine;

public abstract class ControllerObject<T> : ObjectBase where T : ControllerBase
{
    private bool _isRegistered;

    /// <summary>
    /// 컨트롤러 오브젝트의 주인 액터입니다.<para/>
    /// <see cref="Register(T, Action)"/>가 호출 될 때 설정되므로 Initialize 시점에 사용하지 마세요.
    /// </summary>
    protected Actor Author { get; private set; }

    /// <summary>
    /// 컨트롤러 오브젝트의 부모 컨트롤러입니다.<para/>
    /// <see cref="Register(T, Action)"/>가 호출 될 때 설정되므로 Initialize 시점에 사용하지 마세요.
    /// </summary>
    protected T Controller { get; private set; }

    protected sealed override void Awake()
    {
        base.Awake();
        OnInitialized();
        gameObject.SetActive(false);
    }

    protected sealed override void Start()
    {
        base.Start();
    }

    protected sealed override void Update()
    {
        base.Update();
    }

    public sealed override void OnEnable()
    {
        base.OnEnable();
    }


    public sealed override void OnDisable()
    {
        base.OnDisable();
    }

    /// <summary>
    /// 버프 오브젝트나 스킬 오브젝트가 충돌로 갑자기 삭제될 경우에 해제해야 하는 것들은 여기서 구현
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public bool Register(T controller, Action continueFunc)
    {
        if (_isRegistered)
        {
            return false;
        }

        Controller = controller;
        Author = Controller.ControllerManager.GetActor();

        if (Author.photonView.IsMine)
        {
            Controller.PlayerHitEnterEvent += OnPlayerHitEnter;
        }

        _isRegistered = true;
        OnRegistered(continueFunc);
        return true;
    }

    public virtual void Release()
    {
        if (!_isRegistered)
        {
            return;
        }

        if (Author.photonView.IsMine)
        {
            Controller.PlayerHitEnterEvent -= OnPlayerHitEnter;
        }

        _isRegistered = false;
        OnReleased();
    }

    /// <summary>
    /// 해당 오브젝트가 최초로 생성되었을 때 호출됩니다.<para/>
    /// 해당 컨트롤러 오브젝트가 본인 것이 아니더라도 호출됩니다.
    /// </summary>
    protected virtual void OnInitialized()
    {
    }

    /// <summary>
    /// 컨트롤러 오브젝트의 Register가 호출된 후에 실행됩니다.<para/>
    /// 해당 컨트롤러 오브젝트가 본인 것이 아니더라도 호출됩니다.
    /// </summary>
    protected virtual void OnRegistered(Action continueFunc)
    {
    }

    /// <summary>
    /// 컨트롤러 오브젝트가 제거될 때 실행됩니다.<para/>
    /// 해당 컨트롤러 오브젝트가 본인 것이 아니더라도 호출됩니다.
    /// </summary>
    protected virtual void OnReleased()
    {
    }

    /// <summary>
    /// 플레이어가 아무 오브젝트와 충돌이 처음 일어날 때만 무조건 실행되는 함수입니다.<para/>
    /// 해당 컨트롤러 오브젝트가 본인 것일 때만 호출됩니다.
    /// </summary>
    /// <param name="other">충돌이 일어난 상대 오브젝트(게임 오브젝트면 무조건 다 받아옵니다.)</param>
    public virtual void OnPlayerHitEnter(GameObject other)
    {
    }
}
