using UnityEngine;

public class APlayer : ACharacter
{
    [SerializeField]
    [Tooltip("직접 컴포넌트를 넣어주세요.")]
    private Rigidbody _rigidbody;

    private CameraMovement _cameraMovement;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _rigidbody.isKinematic = !photonView.IsMine;
        _cameraMovement = Camera.main.gameObject.GetComponent<CameraMovement>();
        _cameraMovement.AddTarget(transform); // 카메라 타겟 추가 설정
    }

    private void OnDestroy()
    {
        _cameraMovement.RemoveTarget(transform); // 카메라 타겟 삭제
    }
}
