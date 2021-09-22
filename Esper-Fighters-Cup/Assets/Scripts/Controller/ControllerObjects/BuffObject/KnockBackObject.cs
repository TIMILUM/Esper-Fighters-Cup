using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackObject : BuffObject
{
    [SerializeField, Tooltip("넉백 방향 입니다.")]
    private Vector3 _normalizedDirection = Vector3.zero;
    public Vector3 NormalizedDirection
    {
        set => _normalizedDirection = value;
    }

    [SerializeField, Tooltip("최종 넉백 거리는 (스피드 * 지속시간) 입니다.")]
    private float _speed = 1;
    public float Speed
    {
        set => _speed = value;
    }

    private Vector3 _startPosition;
    private Vector3 _endPosition;

    private Actor _actor = null;
    private Rigidbody _rigidbody = null;
    
    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        _normalizedDirection = _normalizedDirection.normalized;
        _actor = _controller.ControllerManager.GetActor();
        _rigidbody = _actor.GetComponent<Rigidbody>();
        _startPosition = _rigidbody.position;
        _endPosition = _startPosition + (_normalizedDirection * _speed * _buffStruct.Duration);
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
        if (photonView != null && !photonView.IsMine) return;
        
        _rigidbody.position += (_normalizedDirection * _speed * Time.deltaTime);
    }

    public override void SetBuffStruct(BuffStruct buffStruct)
    {
        base.SetBuffStruct(buffStruct);
        _normalizedDirection = buffStruct.ValueVector3[0];
        _speed = buffStruct.ValueFloat[0];
    }

    private void Reset()
    {
        _name = "";
        _buffStruct.Type = Type.KnockBack;
    }

    protected override void OnHit(ObjectBase @from, ObjectBase to, BuffStruct[] appendBuff)
    {
        throw new NotImplementedException();
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
        var actor = other.GetComponent<Actor>();
        if (actor == null && !other.CompareTag("Wall")) return;

        var myController =
            _actor.ControllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
        var otherController =
            actor?.ControllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);

        // TODO: 데미지도 추가 예정
        myController?.GenerateBuff(Type.Stun);
        otherController?.GenerateBuff(Type.Stun);
        
        ControllerCast<BuffController>().ReleaseBuff(this);
    }
}
