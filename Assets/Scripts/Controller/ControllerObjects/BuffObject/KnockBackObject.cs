using System;
using UnityEngine;

public class KnockBackObject : BuffObject
{
    [SerializeField]
    [Tooltip("넉백 방향 입니다.")]
    private Vector3 _normalizedDirection = Vector3.zero;

    [SerializeField]
    [Tooltip("최종 넉백 거리는 (스피드 * 지속시간) 입니다.")]
    private float _speed = 1;

    private Actor _actor;
    private Vector3 _endPosition;
    private Rigidbody _rigidbody;

    private Vector3 _startPosition;

    public Vector3 NormalizedDirection
    {
        set => _normalizedDirection = value;
    }

    public float Speed
    {
        set => _speed = value;
    }

    private void Reset()
    {
        _name = "";
        _buffStruct.Type = Type.KnockBack;
    }

    // Start is called before the first frame update
    private new void Start()
    {
        base.Start();
        _normalizedDirection = _normalizedDirection.normalized;
        _actor = _controller.ControllerManager.GetActor();
        _rigidbody = _actor.GetComponent<Rigidbody>();
        _startPosition = _rigidbody.position;
        _endPosition = _startPosition + (_buffStruct.Duration * _speed * _normalizedDirection);
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
        if (photonView != null && !photonView.IsMine)
        {
            return;
        }

        _rigidbody.position += _speed * Time.deltaTime * _normalizedDirection;
    }

    public override void SetBuffStruct(BuffStruct buffStruct)
    {
        base.SetBuffStruct(buffStruct);
        _normalizedDirection = buffStruct.ValueVector3[0];
        _speed = buffStruct.ValueFloat[0];
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffStruct[] appendBuff)
    {
        throw new NotImplementedException();
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
        if (!other.TryGetComponent(out Actor otherActor) && !other.CompareTag("Wall"))
        {
            return;
        }

        if (_actor.ControllerManager.TryGetController(
            ControllerManager.Type.BuffController, out BuffController myController))
        {
            myController.GenerateBuff(Type.Stun);
            myController.ReleaseBuff(this);
        }

        if (otherActor is null)
        {
            return;
        }

        if (otherActor.ControllerManager.TryGetController(
            ControllerManager.Type.BuffController, out BuffController otherController))
        {
            otherController.GenerateBuff(Type.Stun);
        }
    }
}
