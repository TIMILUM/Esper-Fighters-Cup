using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class MovementController : ControllerBase
{
    private APlayer _player = null;
    private BuffController _buffController = null;
    private void Reset()
    {
        // 컨트롤러 타입 지정을 위해 Awake 함수로 이렇게 선언을 해줘야 합니다.
        // 리플렉션으로 전환할 예정 (IL2CPP 모듈 추가가 필요하기 때문에 나중에 전환할 예정)
        SetControllerType(ControllerManager.Type.MovementController);
    }

    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        _player = _controllerManager.GetActor() as APlayer;
        _buffController =
            _controllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
        if (photonView != null && !photonView.IsMine) return;
        
        UpdateMine();
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
        
        // E를 누르면 0.5초 동안 오른쪽으로 넉백이 임시로 걸립니다.
        if (Input.GetKey(KeyCode.E) && _buffController.GetBuff(BuffObject.Type.KnockBack) == null)
        {
            var knockBack = _buffController.GenerateBuff(BuffObject.Type.KnockBack) as KnockBackObject;
            knockBack.Duration = 0.5f;
            knockBack.NormalizedDirection = Vector3.right;
            knockBack.Speed = 3.0f;
        }

        // 스턴 확인 시 움직임을 멈춥니다.
        if (_buffController.GetBuff(BuffObject.Type.Stun) != null)
        {
            return;
        }
        
        // 움직이는지 알아보기 위해 인터넷에서 그냥 움직이는 코드 그대로 복붙한 임시 코드입니다. ====
        var playerPosition = _player.transform.position;
        var moveSpeed = 8;

        if (Input.GetKey(KeyCode.W))
        {
            playerPosition += Vector3.forward * moveSpeed * Time.deltaTime;
            
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerPosition += Vector3.left * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerPosition -= Vector3.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerPosition -= Vector3.left * moveSpeed * Time.deltaTime;
        }
        
        _player.transform.position = playerPosition;
        
        // ====================================================================================
    }
}
