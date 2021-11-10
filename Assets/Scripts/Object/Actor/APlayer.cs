using FMODUnity;
using Photon.Pun;
using UnityEngine;

public class APlayer : ACharacter, IPunObservable
{
    private Rigidbody _rigidbody;

    /**
     * @Todo 임시로 넣은 파라미터입니다. 중간 평가 때문에 급하게 만든 것이니 반드시 제거를 해야함니다
     */
    [Header("임시로 넣은 파라미터입니다. 중간 평가 때문에 급하게 만든 것이니 반드시 제거를 해야함니다")]
    [SerializeField]
    private Transform _hpDummy;

    private CameraMovement _cameraMovement;

    /**
     * @Todo 임시로 넣은 파라미터입니다. 중간 평가 때문에 급하게 만든 것이니 반드시 제거를 해야함니다
     */
    public override float Hp
    {
        get => _hpDummy.transform.localPosition.x;
        set => _hpDummy.transform.localPosition = new Vector3(value, 0, 0);
    }

    protected override void Awake()
    {
        base.Awake();
        IngameFSMSystem.SetPlayer(this);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = !photonView.IsMine;
        _cameraMovement = Camera.main.gameObject.GetComponent<CameraMovement>();
        _cameraMovement.AddTarget(transform); // 카메라 타겟 추가 설정

        if (photonView.IsMine)
        {
            Camera.main.GetComponent<StudioListener>().attenuationObject = gameObject;
        }
    }

    private void OnDestroy()
    {
        _cameraMovement.RemoveTarget(transform); // 카메라 타겟 삭제
    }

    // 임시 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Hp);
        }
        else
        {
            Hp = (float)stream.ReceiveNext();
        }
    }
}
