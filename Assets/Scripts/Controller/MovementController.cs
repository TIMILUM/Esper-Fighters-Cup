using UnityEngine;

public class MovementController : ControllerBase
{

    [SerializeField] private bool _isMousePickLookAt = true;

    [SerializeField, Range(0.001f, 30.0f)] private float _moveSpeed;
    [SerializeField, Range(0.001f, 30.0f), Header("속도 0까지 시간(단위는 초)")] private float _decreaseSpeedTime;
    [SerializeField, Range(0.001f, 30.0f), Header("최고 속도 까지 시간(단위는 초)")] private float _increaseSpeedTime;

    private float _currentIncreaseSpeed;
    private float _currentDecreaseSpeed;
    private Vector3 _currentMoveDir;
    private Vector3 _beforeMoveDirection;

    private Vector3 _fragmentPos;

    private APlayer _player = null;
    private BuffController _buffController = null;


    [SerializeField, Range(0.01f, 1.0f)] private float _smoothLookat = 0.1f;


    private void Reset()
    {
        // 컨트롤러 타입 지정을 위해 Reset 함수로 이렇게 선언을 해줘야 합니다.
        // 리플렉션으로 전환할 예정 (IL2CPP 모듈 추가가 필요하기 때문에 나중에 전환할 예정)
        SetControllerType(ControllerManager.Type.MovementController);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _player = _controllerManager.GetActor() as APlayer;
        _buffController = _controllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (photonView != null && !photonView.IsMine)
        {
            return;
        }

        UpdateMine();

    }

    //마우스 바라보기
    private void MousePickLookAt()
    {


        var playerRotation = _player.transform.rotation;
        var playerPosition = _player.transform.position;
        var screentoRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hitinfos = Physics.RaycastAll(screentoRay);


        if (_player.CharacterAnimator.GetCurrentAnimatorStateInfo(1).IsName("Elena_ReverseGravity_A")
    || _player.CharacterAnimator.GetCurrentAnimatorStateInfo(1).IsName("Elena_ReverseGravity_R"))
        {
            var direction = _fragmentPos - playerPosition;
            playerRotation = Quaternion.Lerp(playerRotation, Quaternion.LookRotation(direction), _smoothLookat);
            _player.transform.rotation = playerRotation;
            return;
        }
        if (!_isMousePickLookAt)
        {
            return;
        }

        foreach (var hitinfo in hitinfos)
        {
            if (hitinfo.transform.CompareTag("Floor"))
            {
                var lookAtDirection = hitinfo.point + new Vector3(0.0f, _player.GetComponent<Collider>().bounds.extents.y, 0.0f) - playerPosition;
                playerRotation = Quaternion.Lerp(playerRotation, Quaternion.LookRotation(lookAtDirection), _smoothLookat);


                //바라보는 방향에 맞게 애니메이션 적용
                var tempPos = Vector3.forward.normalized;
                var targetDirctionNormal = lookAtDirection.normalized;

                var crossProduct = Vector3.Cross(tempPos, targetDirctionNormal);
                float sin = Vector3.Dot(crossProduct, Vector3.up);
                float cos = Vector3.Dot(tempPos, targetDirctionNormal);

                _player.CharacterAnimator.SetFloat("Cos", cos);
                _player.CharacterAnimator.SetFloat("Sin", sin);
                _fragmentPos = hitinfo.point;
            }
        }

        _player.transform.rotation = playerRotation;
    }

    //이동 방향에 따라 앵글 변경
    private void DirectionLookAt()
    {
        if (_isMousePickLookAt || _currentMoveDir == Vector3.zero)
        {
            return;
        }

        var playerRotation = _player.transform.rotation;
        playerRotation = Quaternion.Lerp(playerRotation, Quaternion.LookRotation(_beforeMoveDirection), _smoothLookat);
        _player.transform.rotation = playerRotation;
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
    }

    private void UpdateMine()
    {
        // 이 함수의 모든 코드는 모두 임시코드입니다. 잘 돌아가는지 확인해보려고 작성했습니다!

        // Q를 누르면 3초 스턴이 임시로 걸립니다.
        //if (Input.GetKey(KeyCode.Q) && _buffController.GetBuff(BuffObject.Type.Stun) == null)
        //{
        //    var stun = _buffController.GenerateBuff(BuffObject.Type.Stun);
        //    stun.Duration = 3;
        //}




        var dirx = Input.GetAxisRaw("Horizontal");
        var dirz = Input.GetAxisRaw("Vertical");


        var dir = new Vector3(dirx, 0.0f, dirz).normalized;


        // 스턴 및 띄움상태 확인 시 움직임을 멈춥니다.
        if (_buffController.GetBuff(BuffObject.Type.Stun) != null || _buffController.GetBuff(BuffObject.Type.Raise) != null)
        {
            dir = Vector3.zero;
            _currentDecreaseSpeed = 1.0f;
            _currentIncreaseSpeed = 0.0f;

            return;
        }






        var playerPosition = _player.transform.position;

        _player.CharacterAnimator.SetFloat("DirX", dirx);
        _player.CharacterAnimator.SetFloat("DirZ", dirz);

        var currentSpeedTime = 0.0f;

        var tempDirection = Vector3.zero;
        if (dir == Vector3.zero)
        {
            _currentIncreaseSpeed -= Time.deltaTime / _increaseSpeedTime;
            _currentDecreaseSpeed += Time.deltaTime / _decreaseSpeedTime;
            _currentDecreaseSpeed = Mathf.Clamp(_currentDecreaseSpeed, 0.0f, 1.0f);
            _currentIncreaseSpeed = Mathf.Clamp(_currentIncreaseSpeed, 0.0f, 1.0f);
            tempDirection = _beforeMoveDirection;
            currentSpeedTime = _currentDecreaseSpeed;

        }
        else
        {
            _currentDecreaseSpeed -= Time.deltaTime / _decreaseSpeedTime;
            _currentIncreaseSpeed += Time.deltaTime / _increaseSpeedTime;
            _currentIncreaseSpeed = Mathf.Clamp(_currentIncreaseSpeed, 0.0f, 1.0f);
            _currentDecreaseSpeed = Mathf.Clamp(_currentDecreaseSpeed, 0.0f, 1.0f);
            _beforeMoveDirection = _currentMoveDir;
            currentSpeedTime = _currentIncreaseSpeed;
        }

        _currentMoveDir = Vector3.Lerp(tempDirection, dir, currentSpeedTime);

        playerPosition += _moveSpeed * Time.deltaTime * _currentMoveDir;

        _player.transform.position = playerPosition;


        MousePickLookAt();
        DirectionLookAt();
    }

}
