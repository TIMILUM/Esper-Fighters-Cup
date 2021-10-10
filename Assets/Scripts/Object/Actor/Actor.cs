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

    protected BuffController _buffController;
    public ControllerManager ControllerManager => _controllerManager;

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

    private void OnPlayerHitEnter(GameObject other)
    {
        _controllerManager.OnPlayerHitEnter(other);
    }
}
