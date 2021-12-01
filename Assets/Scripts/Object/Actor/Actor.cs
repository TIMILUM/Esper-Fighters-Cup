using FMODUnity;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonRigidbodyView))]
public class Actor : ObjectBase, IPunObservable
{
    [SerializeField]
    [Tooltip("오브젝트를 직접 넣어주세요!")]
    protected ControllerManager _controllerManager;

    [SerializeField, Tooltip("해당 오브젝트의 ID 값입니다.")]
    private int _id;

    [SerializeField]
    private StudioEventEmitter _audioEmitter;

    public ControllerManager ControllerManager => _controllerManager;
    public BuffController BuffController { get; protected set; }
    public int ID => _id;
    public StudioEventEmitter AudioEmitter => _audioEmitter != null ? _audioEmitter : null;
    public Rigidbody Rigidbody { get; protected set; }

    protected override void Awake()
    {
        base.Awake();

        Debug.Assert(_controllerManager, "컨트롤러 매니저가 지정되어 있지 않습니다.");
        ControllerManager.SetActor(this);
        Rigidbody = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        base.Start();

        BuffController = _controllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
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
        if (BuffController == null)
        {
            return;
        }

        foreach (var buffStruct in appendBuff)
        {
            BuffController.GenerateBuff(buffStruct);
        }
    }

    /// <summary>
    /// 플레이어가 아무 오브젝트와 충돌이 처음 일어날 때만 무조건 실행되는 함수입니다.
    /// </summary>
    /// <param name="other">충돌이 일어난 상대 오브젝트(게임 오브젝트면 무조건 다 받아옵니다.)</param>
    protected virtual void OnPlayerHitEnter(GameObject other)
    {
        ControllerManager.OnPlayerHitEnter(other);
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(Rigidbody.isKinematic);
            //stream.SendNext(Rigidbody.useGravity);
        }
        else
        {
            //Rigidbody.isKinematic = (bool)stream.ReceiveNext();
            //Rigidbody.useGravity = (bool)stream.ReceiveNext();
        }
    }
}
