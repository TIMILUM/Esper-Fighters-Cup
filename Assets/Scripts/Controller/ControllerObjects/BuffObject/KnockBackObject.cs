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

    // 넉백 후 오브젝트와 충돌 시 해당값 만큼 HP가 줄어듬
    private float _decreaseHp = 0;
    
    // 넉백 후 오브젝트와 충돌 시 해당값 만큼 스턴 지속기간(초) 지정됨
    private float _durationStunSeconds = 0;

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
        // BuffStruct Help
        // ---------------
        // ValueVector3[0]  : _normalizedDirection
        // ---------------
        // ValueFloat[0]    : _speed
        // ValueFloat[1]    : _decreaseHp (0이면 HP감소 효과 없음)
        // ValueFloat[2]    : _durationStunSeconds (0이면 스턴 효과 없음)
        // ---------------
        
        base.SetBuffStruct(buffStruct);
        _normalizedDirection = buffStruct.ValueVector3[0];
        _speed = buffStruct.ValueFloat[0];
        _decreaseHp = buffStruct.ValueFloat[1];
        _durationStunSeconds = buffStruct.ValueFloat[2];
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
            _rigidbody.velocity = -_normalizedDirection * 2; // 넉백 후 충돌로 인한 튕기는 효과 추가
            GenerateAfterBuff(myController);
            myController.ReleaseBuff(this);
        }

        if (otherActor is null)
        {
            return;
        }

        if (otherActor.ControllerManager.TryGetController(
            ControllerManager.Type.BuffController, out BuffController otherController))
        {
            GenerateAfterBuff(otherController);
        }
    }

    // 넉백 후 오브젝트와 충돌 시 추가적으로 얻는 버프를 이 함수에서 생성함
    private void GenerateAfterBuff(BuffController controller)
    {
        if (_decreaseHp > 0)
        {
            controller.GenerateBuff(new BuffStruct()
            {
                Type = Type.DecreaseHp,
                Damage = _decreaseHp,
                Duration = 0.001f
            });
        }
        
        if (_durationStunSeconds > 0)
        {
            controller.GenerateBuff(new BuffStruct()
            {
                Type = Type.Stun,
                Duration = _durationStunSeconds
            });
        }
    }
}
