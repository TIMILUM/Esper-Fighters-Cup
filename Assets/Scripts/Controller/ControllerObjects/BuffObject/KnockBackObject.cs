using UnityEngine;

public class KnockBackObject : BuffObject
{
    [SerializeField]
    [Tooltip("넉백 방향 입니다.")]
    private Vector3 _normalizedDirection = Vector3.zero;
    private ACharacter _character;

    [SerializeField]
    [Tooltip("최종 넉백 거리는 (스피드 * 지속시간) 입니다.")]
    private float _speed = 1;

    // private Vector3 _endPosition;
    // private Vector3 _startPosition;
    private Rigidbody _rigidbody;

    // 넉백 후 오브젝트와 충돌 시 해당값 만큼 HP가 줄어듬
    private float _decreaseHp = 0;

    // 넉백 후 오브젝트와 충돌 시 해당값 만큼 스턴 지속기간(초) 지정됨
    private float _durationStunSeconds = 0;

    public Vector3 NormalizedDirection
    {
        get => _normalizedDirection;
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

        _character = Author as ACharacter;

        if (!(_character is null))
        {
            _character.CharacterAnimatorSync.SetTrigger("Knockback");
        }
    }

    protected override void OnRegistered()
    {
        _normalizedDirection = _normalizedDirection.normalized;
        _rigidbody = Author.GetComponent<Rigidbody>();
    }

    protected override void Update()
    {
        base.Update();
        if (!IsRegistered || !Author.photonView.IsMine)
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
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
        if (!other.TryGetComponent(out Actor otherActor) && !other.CompareTag("Wall"))
        {
            return;
        }

        if (Author.ControllerManager.TryGetController(
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
