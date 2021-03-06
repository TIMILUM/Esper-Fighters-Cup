using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public abstract class ControllerBase : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private ControllerManager.Type _type = ControllerManager.Type.None;

    public ControllerManager ControllerManager { get; private set; }

    private UnityAction<GameObject> _onPlayerHitEnterEvent = null;
    public event UnityAction<GameObject> PlayerHitEnterEvent
    {
        add => _onPlayerHitEnterEvent += value;
        remove => _onPlayerHitEnterEvent -= value;
    }

    protected virtual void Reset()
    {
    }

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    public void Register(ControllerManager controllerManager)
    {
        ControllerManager = controllerManager;
        ControllerManager.RegisterController(_type, this);
    }

    protected virtual void SetControllerType(ControllerManager.Type type)
    {
        _type = type;
    }

    /// <summary>
    ///     플레이어가 아무 오브젝트와 충돌이 처음 일어날 때만 무조건 실행되는 함수입니다.
    /// </summary>
    /// <param name="other">충돌이 일어난 상대 오브젝트(게임 오브젝트면 무조건 다 받아옵니다.)</param>
    public virtual void OnPlayerHitEnter(GameObject other)
    {
        _onPlayerHitEnterEvent?.Invoke(other);
    }
}
