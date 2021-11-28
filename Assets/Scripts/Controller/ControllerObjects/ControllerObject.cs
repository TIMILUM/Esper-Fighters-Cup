using UnityEngine;

public abstract class ControllerObject<T> : ObjectBase where T : ControllerBase
{
    private bool _isRegistered;

    /// <summary>
    /// 컨트롤러 오브젝트의 주인 액터입니다.
    /// </summary>
    protected Actor Author { get; private set; }
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

    protected sealed override void OnDestroy()
    {
        base.OnDestroy();
    }

    public sealed override void OnEnable()
    {
        base.OnEnable();
    }

    public sealed override void OnDisable()
    {
        base.OnDisable();
    }

    public void Register(T controller)
    {
        if (_isRegistered)
        {
            return;
        }

        Controller = controller;
        Author = Controller.ControllerManager.GetActor();

        if (Author.photonView.IsMine)
        {
            Controller.PlayerHitEnterEvent += OnPlayerHitEnter;
        }

        _isRegistered = true;
        OnRegistered();
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
    protected virtual void OnRegistered()
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
