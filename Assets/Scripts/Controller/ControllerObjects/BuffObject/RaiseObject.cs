using UnityEngine;

public class RaiseObject : BuffObject
{
    [SerializeField, Tooltip("높이 데이터 입니다.")]
    private float _raiseDate;

    private Actor _actor;
    private Rigidbody _rigidbody;
    private float _limitPosy;
    private Vector3 _startPos;
    private float _startTime;
    private float _endTime;



    private void Reset()
    {
        _name = "";
        _buffStruct.Type = Type.Raise;
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

    protected override void Update()
    {
        base.Update();
        _endTime = Time.time;
        float currentTime = _endTime - _startTime;

        var buff = _actor.BuffController.GetBuff(BuffObject.Type.KnockBack);
        if (buff != null)
        {
            GenerateAfterBuff(_actor.BuffController);
            _actor.BuffController.ReleaseBuff(this);

        }
        else
        {
            _actor.transform.position = Vector3.Lerp(_startPos, new Vector3(_actor.transform.position.x,
    _limitPosy, _actor.transform.position.z), currentTime / Duration);
        }
    }


    private void GenerateAfterBuff(BuffController controller)
    {

        Vector3 dir = GetMousePosition() - transform.position;
        float[] FloatValue = new float[3];
        Vector3[] Vector = new Vector3[1];
        Vector[0] = dir;
        FloatValue[0] = 5;
        FloatValue[1] = 10;
        FloatValue[2] = 0.5f;



        controller.GenerateBuff(new BuffStruct()
        {
            Type = Type.KnockBack,
            Duration = 0.3f,
            ValueFloat = FloatValue,
            ValueVector3 = Vector,
        });


    }



    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_rigidbody != null)
        {
            _rigidbody.useGravity = true;
        }
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
