using FMODUnity;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonRigidbodyView))]
[RequireComponent(typeof(StudioEventEmitter))]
public class Actor : ObjectBase
{
    [SerializeField]
    [Tooltip("오브젝트를 직접 넣어주세요!")]
    protected ControllerManager _controllerManager;
    public ControllerManager ControllerManager => _controllerManager;

    protected BuffController _buffController;
    public BuffController BuffController => _buffController;

    [SerializeField]
    private float _hp;
    public float Hp
    {
        get => _hp;
        set => _hp = value;
    }

    [SerializeField, Tooltip("해당 오브젝트의 ID 값입니다.")]
    private int _id;
    public int ID => _id;

    public StudioEventEmitter AudioEmitter { get; private set; }

    protected virtual void Awake()
    {
        Debug.Assert(_controllerManager, "컨트롤러 매니저가 지정되어 있지 않습니다.");
        _controllerManager.SetActor(this);

        AudioEmitter = GetComponent<StudioEventEmitter>();
    }

    protected virtual void Start()
    {
        _buffController = _controllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
    }

    private void OnCollisionEnter(Collision other)
    {
        OnPlayerHitEnter(other.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnPlayerHitEnter(other.gameObject);
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
        if (_buffController == null)
        {
            return;
        }

        foreach (var buffStruct in appendBuff)
        {
            _buffController.GenerateBuff(buffStruct);
        }
    }

    /// <summary>
    /// 플레이어가 아무 오브젝트와 충돌이 처음 일어날 때만 무조건 실행되는 함수입니다.
    /// </summary>
    /// <param name="other">충돌이 일어난 상대 오브젝트(게임 오브젝트면 무조건 다 받아옵니다.)</param>
    protected virtual void OnPlayerHitEnter(GameObject other)
    {
        _controllerManager.OnPlayerHitEnter(other);
    }
}
