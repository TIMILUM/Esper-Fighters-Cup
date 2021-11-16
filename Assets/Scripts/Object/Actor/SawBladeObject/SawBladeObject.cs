using EsperFightersCup.Net;
using UnityEngine;

public class SawBladeObject : AStaticObject
{
    [Header("[[ ※ 레벨링 시 금지 사항 : 끝지점 포지션을 절대 벽 모서리 근처에 두지 마세요! ]]")]
    [SerializeField]
    private BuffObject.Type[] _allowBuffList;

    [SerializeField]
    private ColliderChecker _collider;

    [SerializeField]
    private float _speed = 3;

    [SerializeField]
    private AnimatorSync _animator = null;

    public float Speed => _speed;
    public AnimatorSync SawBladeAnimator => _animator;
    public Vector3 Direction { get; private set; } = Vector3.one;
    public Transform StartPosition { get; private set; }
    public Transform EndPosition { get; private set; }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _collider.OnCollision += SetHit;
        GetComponent<Rigidbody>().useGravity = false;

        if (photonView.IsMine)
        {
            SawBladeSystem.Instance.LocalSpawnedSawBlades[photonView.ViewID] = this;
        }
    }

    protected virtual void OnDestroy()
    {
        if (photonView.IsMine)
        {
            SawBladeSystem.Instance.LocalSpawnedSawBlades.Remove(photonView.ViewID);
        }
    }

    public override void SetHit(ObjectBase to)
    {
        var hitDirection = Vector3.Normalize(transform.position - to.transform.position);
        _buffOnCollision[0].ValueVector3[0] = -hitDirection;
        base.SetHit(to);
    }

    public void SetDirection(Transform start, Transform end)
    {
        StartPosition = start;
        EndPosition = end;
        var startPosition = StartPosition.position;
        Direction = Vector3.Normalize(EndPosition.position - startPosition);

        transform.SetPositionAndRotation(startPosition, Quaternion.LookRotation(Direction));
    }

    protected override void OnHit(ObjectBase @from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
        if (BuffController == null)
        {
            return;
        }

        foreach (var buffStruct in appendBuff)
        {
            if (IsAllowBuff(buffStruct))
            {
                BuffController.GenerateBuff(buffStruct);
            }
        }
    }

    private bool IsAllowBuff(BuffObject.BuffStruct buffStruct)
    {
        foreach (var type in _allowBuffList)
        {
            if (buffStruct.Type == type)
            {
                return true;
            }
        }

        return false;
    }
}
