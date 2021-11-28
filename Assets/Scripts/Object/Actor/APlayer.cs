using System.Collections.Generic;
using FMODUnity;
using Photon.Pun;
using UnityEngine;

public class APlayer : ACharacter, IPunObservable, IPunInstantiateMagicCallback
{

    // 이펙트가 손위치에 나오는 기획이 있어서 추가합니다.
    [SerializeField]
    private List<Transform> _effectTrans;


    private Rigidbody _rigidbody;
    private CameraMovement _cameraMovement;

    public List<Transform> EffectTrans => _effectTrans;


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

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _cameraMovement.RemoveTarget(transform); // 카메라 타겟 삭제
    }

    public void ResetPositionAndRotation()
    {
        var idx = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        var startLocation = InGamePlayerManager.Instance.StartLocations[idx];
        transform.SetPositionAndRotation(startLocation.position, startLocation.rotation);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = this;
        gameObject.name = $"{info.Sender.NickName}_{info.Sender.ActorNumber}";
        Debug.Log($"Set {info.Sender.NickName}'s TagObject to {gameObject}");
    }
}
