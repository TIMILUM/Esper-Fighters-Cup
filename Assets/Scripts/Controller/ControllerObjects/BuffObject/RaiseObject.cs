using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseObject : BuffObject
{
    [SerializeField, Tooltip("높이 데이터 입니다.")]
    private float _raiseDate;

    [SerializeField]
    [Tooltip("올라갈 스피드 입니다.")]
    private float _speed = 1;
    private Actor _actor;
    private Vector3 _endPoint;

    private Rigidbody _rigidbody;

    private void Reset()
    {
        _name = "";
        _buffStruct.Type = Type.Raise;

    }

    private new void Start()
    {
        base.Start();
        _actor = _controller.ControllerManager.GetActor();
        _rigidbody = _actor.GetComponent<Rigidbody>();
        _endPoint = transform.position + new Vector3(0.0f, _raiseDate, 0.0f);
        _rigidbody.useGravity = false;

    }


    public override void SetBuffStruct(BuffStruct buffStruct)
    {
        base.SetBuffStruct(buffStruct);
        _speed = buffStruct.ValueFloat[0];
        _raiseDate = buffStruct.ValueFloat[1];
        _buffStruct.Duration = buffStruct.ValueFloat[2];
    }

    private void OnDestroy()
    {
        _rigidbody.useGravity = true;
    }
    protected new void Update()
    {
        base.Update();
        if (_endPoint.y > _actor.transform.position.y)
            _rigidbody.position += Vector3.up * _speed * Time.deltaTime;



    }
    public override void OnPlayerHitEnter(GameObject other)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffStruct[] appendBuff)
    {
        //throw new System.NotImplementedException();
    }
}
