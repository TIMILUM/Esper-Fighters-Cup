using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBladeObject : AStaticObject
{
    /**
     * @Todo FSM을 통해 톱날의 상태변화를 처리할 수 있도록 수정이 필요합니다. 현재 톱날 시스템의 기본적인 구현만 작성되었습니다.
     */

    [SerializeField]
    private BuffObject.Type[] _allowBuffList;

    [SerializeField]
    private ColliderChecker _collider;

    [SerializeField]
    private float _speed = 3;

    private Vector3 _direction = Vector3.one;

    private Transform _startPosition;
    private Transform _endPosition;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _collider.OnCollision += SetHit;
    }

    // Update is called once per frame
    private void Update()
    {
        // FSM으로 따로 업데이트 함수가 진행될 예정 (임시코드입니다)

        if (_direction == Vector3.one)
        {
            return;
        }

        var position = transform.position;
        position += _direction * _speed * Time.deltaTime;
        transform.position = position;

        // 벽과 충돌하면 움직임이 멈추는 코드를 임시로 작성하였습니다.
        // 해당 코드는 FSM을 통해 수정될 예정입니다.

        var endDistance = Vector3.Distance(_endPosition.position, position);
        if(endDistance < 0.5f)
        {
            _direction = Vector3.one;
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
