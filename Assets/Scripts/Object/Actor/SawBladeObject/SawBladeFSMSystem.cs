using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SawBladeObject))]
public class SawBladeFSMSystem : InspectorFSMSystem<SawBladeFSMSystem.StateEnum, SawBladeFSMBase>
{
    public enum StateEnum
    {
        Move,
        OnLift,
        HitWall,
    }

    [SerializeField, Tooltip("상태변화에 필요한 충돌체크입니다. 플레이어 피격 판정의 충돌체크가 아닙니다!")]
    private ColliderChecker _colliderCenter = null;

    [SerializeField, Tooltip("버프 컨트롤러를 직접 넣어주시면 됩니다.")]
    private BuffController _buffController = null;

    public BuffController BuffControllerObject => _buffController;

    private Vector3 _direction;
    public Vector3 Direction => _direction;

    private void Start()
    {
        if (!_buffController.photonView.IsMine)
        {
            return;
        }
    }

    private void Update()
    {
        if (!_buffController.photonView.IsMine)
        {
            return;
        }

        // 넉백 버프가 있으면 방향을 추출하고 OnLift상태로 변경 후 넉백버프 제거
        var knockBackList = _buffController.GetBuff(BuffObject.Type.KnockBack);
        if (knockBackList != null)
        {
            _direction = ((KnockBackObject)knockBackList[0]).NormalizedDirection;
            _buffController.ReleaseBuff(BuffObject.Type.KnockBack);
            ChangeState(StateEnum.OnLift);
        }
    }
}
