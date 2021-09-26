using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class MovementController : ControllerBase
{

    [SerializeField] private bool _isMousePickLooking;

    [SerializeField, Range(0.001f, 30.0f)] private float _moveSpeed;
    [SerializeField, Range(0.001f, 30.0f), Header("속도 0까지 시간(단위는 초)")] private float _decreaseSpeedTime;
    [SerializeField, Range(0.001f, 30.0f), Header("최고 속도 까지 시간(단위는 초)")] private float _increaseSpeedTime;

    private float _currentIncreaseSpeed; 
    private float _currentDecreaseSpeed;
    private Vector3 _currentMoveDir;
    private Vector3 _beforeMoveDirection;

    private APlayer _player = null;
    private BuffController _buffController = null;


    [SerializeField , Range(0.01f ,1.0f)]private float _smoothLookat = 0.3f;
    

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
        MousePickLookAt();
        DirectionLookAt();
    }

    private void MousePickLookAt()
    {
        if (!_isMousePickLooking)
        {
            return;
        }

        var playerRotation = _player.transform.rotation;
        var playerPosition = _player.transform.position;
        var ScreentoRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hitinfos = Physics.RaycastAll(ScreentoRay);

        foreach (var hitinfo in hitinfos)
        {
            if (hitinfo.transform.tag == "Floor")
            {
                var LookAtDirection = (hitinfo.point + new Vector3(0.0f, _player.GetComponent<Collider>().bounds.extents.y, 0.0f)) - (playerPosition);
                playerRotation = Quaternion.Lerp(playerRotation, Quaternion.LookRotation(LookAtDirection), _smoothLookat);
            }
        }

        _player.transform.rotation = playerRotation;




    }

    private void DirectionLookAt()
    {
        if (_isMousePickLooking) return;
        if (_currentMoveDir == Vector3.zero) return;

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
        if (Input.GetKey(KeyCode.Q) && _buffController.GetBuff(BuffObject.Type.Stun) == null)
        {
            var stun = _buffController.GenerateBuff(BuffObject.Type.Stun);
            stun.Duration = 3;
        }

        // // E를 누르면 0.5초 동안 오른쪽으로 넉백이 임시로 걸립니다.
        // if (Input.GetKey(KeyCode.E) && _buffController.GetBuff(BuffObject.Type.KnockBack) == null)
        // {
        //     var knockBack = _buffController.GenerateBuff(BuffObject.Type.KnockBack) as KnockBackObject;
        //     knockBack.Duration = 0.5f;
        //     knockBack.NormalizedDirection = Vector3.right;
        //     knockBack.Speed = 3.0f;
        // }

        // 스턴 확인 시 움직임을 멈춥니다.
        if (_buffController.GetBuff(BuffObject.Type.Stun) != null)
        {
            return;
        }

        var playerPosition = _player.transform.position;



        float dirx = Input.GetAxisRaw("Horizontal");
        float dirz = Input.GetAxisRaw("Vertical");

        var dir = new Vector3(dirx, 0.0f, dirz).normalized;


        float currentSpeedTime = 0.0f;
        var tempDirection = Vector3.zero;
        if (dir == Vector3.zero)
        {
            _currentIncreaseSpeed = 0.0f;
            _currentDecreaseSpeed += Time.deltaTime / _decreaseSpeedTime;
            _currentDecreaseSpeed = Mathf.Clamp(_currentDecreaseSpeed, 0.0f, 1.0f);
            tempDirection = _beforeMoveDirection;
            currentSpeedTime = _currentDecreaseSpeed;

        }
        else
        {
            _currentDecreaseSpeed = 0.0f;
            _currentIncreaseSpeed += Time.deltaTime / _increaseSpeedTime;
            _currentIncreaseSpeed = Mathf.Clamp(_currentIncreaseSpeed, 0.0f, 1.0f);
            _beforeMoveDirection = _currentMoveDir;
            currentSpeedTime = _currentIncreaseSpeed;
        }

        _currentMoveDir = Vector3.Lerp(tempDirection, dir, currentSpeedTime);

        playerPosition += _currentMoveDir * Time.deltaTime * _moveSpeed;

        _player.transform.position = playerPosition;
    }

}
