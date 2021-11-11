using UnityEngine;

public class RaiseObject : BuffObject
{
    [SerializeField]
    [Tooltip("높이 데이터 입니다.")]
    private float _raiseDate;

    private Actor _actor;
    private float _endTime;
    private float _limitPosy;
    private Rigidbody _rigidbody;
    private Vector3 _startPos;
    private float _startTime;

    private void Reset()
    {
        _name = "";
        _buffStruct.Type = Type.Raise;
    }

    protected override void Update()
    {
        base.Update();
        _endTime = Time.time;
        var currentTime = _endTime - _startTime;

        var buff = _actor.BuffController.GetBuff(Type.KnockBack);
        if (buff == null)
        {
            _actor.transform.position = Vector3.Lerp(
                _startPos, new Vector3(_actor.transform.position.x, _limitPosy, _actor.transform.position.z),
                currentTime / Duration);
        }
        else
        {
            _actor.BuffController.ReleaseBuff(this);
        }
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_rigidbody != null)
        {
            _rigidbody.useGravity = true;
        }
    }

    protected override void OnRegistered()
    {
        base.OnRegistered();

        _actor = Controller.ControllerManager.GetActor();
        _rigidbody = _actor.GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _startPos = _actor.transform.position;
        _startTime = Time.time;
    }

    public override void SetBuffStruct(BuffStruct buffStruct)
    {
        // BuffStruct Help
        // ----------------
        // ValueFloat[0] : limitPosY (0이면 스턴 효과 없음)
        // ----------------

        base.SetBuffStruct(buffStruct);
        _limitPosy = buffStruct.ValueFloat[0];
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffStruct[] appendBuff)
    {
    }

    private Vector3 GetMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Floor"))
            {
                return hit.point;
            }
        }

        return Vector3.positiveInfinity;
    }
}
