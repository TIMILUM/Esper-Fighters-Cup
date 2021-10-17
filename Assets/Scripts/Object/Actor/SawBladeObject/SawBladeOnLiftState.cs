using UnityEngine;

public class SawBladeOnLiftState : SawBladeFSMBase
{
    [SerializeField]
    [Tooltip("해당 상태인 동안 y축으로 띄워올려질 스칼라 값입니다.")]
    private float _yUpScalar = 1;

    [SerializeField]
    [Tooltip("하위 오브젝트 중 Transform 오브젝트를 넣어주면 됩니다.")]
    private Transform _transformObject;

    [SerializeField]
    [Tooltip("톱날이 날아가는 속도입니다.")]
    private float _speed = 4;

    [SerializeField]
    [Tooltip("벽에 박혀있는걸 확인하기위해 사용되는 레이케스트의 최소범위입니다.")]
    private float _rayMinDistance = 0.4f;

    private Vector3 _endPosition;

    // 최종적인 상태에 따라 Transform 오브젝트를 아래의 result변수의 값으로 설정됩니다.
    private Vector3 _resultPosition;
    private Quaternion _resultRotation;

    private Rigidbody _rigidbody;

    protected override void Start()
    {
        base.Start();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        var direction = FsmSystem.Direction;
        if (direction == Vector3.one)
        {
            ChangeState(SawBladeFSMSystem.StateEnum.HitWall);
            return;
        }
        var position = _rigidbody.position;

        // 끝지점에 다다르면 움직임을 종료하고 HitWall상태로 전환
        var endDistance = Vector3.Distance(_endPosition, position);
        if (endDistance < 0.5f)
        {
            ChangeState(SawBladeFSMSystem.StateEnum.HitWall);
            return;
        }

        position += direction * _sawBladeObject.Speed * Time.deltaTime;
        _rigidbody.position = position;
    }

    protected override void Initialize()
    {
        State = SawBladeFSMSystem.StateEnum.OnLift;
    }

    public override void StartState()
    {
        if (!GetEndPosition(FsmSystem.Direction)) // 레이 케스트를 통해 날아가는 방향에 벽이 존재하는지 확인
        {
            ChangeState(SawBladeFSMSystem.StateEnum.HitWall);
            return;
        }

        _transformObject.localPosition = new Vector3(0, _yUpScalar, 0);
        _transformObject.localRotation = Quaternion.Euler(0, 0, 90);
        transform.rotation = Quaternion.LookRotation(FsmSystem.Direction);
    }

    public override void EndState()
    {
    }

    private bool GetEndPosition(Vector3 direction)
    {
        var rayList = Physics.RaycastAll(transform.position, direction, 1000);
        foreach (var ray in rayList)
        {
            if (ray.distance < _rayMinDistance) // 거리가 너무 가까우면 제외
            {
                continue;
            }

            if (!ray.collider.CompareTag("Wall")) // 오브젝트 tag가 Wall이 아닐경우 제외
            {
                continue;
            }

            _endPosition = ray.point;
            return true;
        }

        return false;
    }
}
