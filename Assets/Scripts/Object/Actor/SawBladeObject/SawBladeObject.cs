using System.Collections;
using System.Collections.Generic;
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
    public float Speed => _speed;

    private Vector3 _direction = Vector3.one;
    public Vector3 Direction => _direction;

    private Transform _startPosition;
    private Transform _endPosition;
    public Transform EndPosition => _endPosition;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _collider.OnCollision += SetHit;
    }

    public override void SetHit(ObjectBase to)
    {
        var hitDirection = Vector3.Normalize(transform.position - to.transform.position);
        _buffOnCollision[0].ValueVector3[0] = -hitDirection;
        base.SetHit(to);
    }
    
    public void SetDirection(Transform start, Transform end)
    {
        _startPosition = start;
        _endPosition = end;
        var startPosition = _startPosition.position;
        _direction = Vector3.Normalize(_endPosition.position - startPosition);
        
        transform.SetPositionAndRotation(startPosition, Quaternion.LookRotation(_direction));
    }

    protected override void OnHit(ObjectBase @from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
        if (_buffController == null)
        {
            return;
        }

        foreach (var buffStruct in appendBuff)
        {
            if (IsAllowBuff(buffStruct))
            {
                _buffController.GenerateBuff(buffStruct);
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
