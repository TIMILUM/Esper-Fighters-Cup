using EsperFightersCup;
using FMODUnity;
using Photon.Pun;
using UnityEngine;

public class APlayer : ACharacter, IPunObservable
{
    private Rigidbody _rigidbody;
    private CameraMovement _cameraMovement;

    public int WinPoint
    {
        get
        {
            var value = PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyKeys.PlayerWinPoint];
            return value is int winPoint ? winPoint : 0;
        }
        set
        {
            var localplayer = PhotonNetwork.LocalPlayer;
            if (localplayer != photonView.Owner)
            {
                return;
            }
            localplayer.SetCustomProperties(CustomPropertyKeys.PlayerWinPoint, value);
        }
    }

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

    public void ResetPositionAndRotation()
    {
        var idx = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        var startLocation = InGamePlayerManager.Instance.StartLocations[idx];
        transform.SetPositionAndRotation(startLocation.position, startLocation.rotation);
    }
}
