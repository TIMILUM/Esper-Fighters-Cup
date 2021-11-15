using FMODUnity;
using Photon.Pun;
using UnityEngine;

public class APlayer : ACharacter, IPunObservable
{
    private Rigidbody _rigidbody;
    private CameraMovement _cameraMovement;

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
}
