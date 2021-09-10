using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class MovementController : ControllerBase
{
    private APlayer _player = null;
    private void Awake()
    {
        // 컨트롤러 타입 지정을 위해 Awake 함수로 이렇게 선언을 해줘야 합니다.
        // 리플렉션으로 전환할 예정 (IL2CPP 모듈 추가가 필요하기 때문에 나중에 전환할 예정)
        SetControllerType(ControllerManager.ControllerType.MovementController);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _player = _controllerManager.GetActor() as APlayer;
    }

    // Update is called once per frame
    private void Update()
    {
        if (photonView != null && !photonView.IsMine) return;
        
        
    }
}
