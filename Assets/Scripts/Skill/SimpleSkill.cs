using UnityEngine;


//간단하게 염동력 구현
public class SimpleSkill : SkillBase
{
    [SerializeField] private float _force;
    [SerializeField] private float _rotateSpeed;

    private Transform _targetObj;
    private Vector3 _endPos;
    private float _realTime;

    // 이벤트 적용
    public override void SkillExecute()
    {
        MousePicking();
        StayObject();
    }

    // 마우스 스킬이 끝났는지 판별
    public override bool EndSkill()
    {
        return MouseEndSkill();
    }

    //마우스 피킹
    public void MousePicking()
    {
        if (Vector3.Distance(transform.position, _endPos) > _SKillRange && _targetObj == null)
        {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitinfo))
        {
            if (hitinfo.transform.CompareTag("Object"))
            {
                if (_targetObj == null)
                {
                    _targetObj = hitinfo.transform;
                    _targetObj.transform.GetComponent<Rigidbody>().velocity = Vector3.zero; // 힘 크기를 0으로 시켜줍니다.
                }
            }
            _endPos = hitinfo.point;
        }
    }

    // 스킬 키를 놓았을때 이벤트 적용
    public bool MouseEndSkill()
    {
        if (_SkillKeyCode == SKILLKEYCODE.CONTROL)
        {
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                ResetSkill();
                return true;
            }
        }
        else if (_SkillKeyCode == SKILLKEYCODE.LEFTMOUSEBUTTONDOWN)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                ResetSkill();
                return true;
            }
        }
        else if (_SkillKeyCode == SKILLKEYCODE.RIGHTMOUSEBUTTONDOWN)
        {
            if (Input.GetButtonUp("Fire2"))
            {
                ResetSkill();
                return true;
            }
        }

        return false;
    }

    private void ResetSkill()
    {
        //스킬 끝날을때 리셋
        if (_targetObj != null)
        {
            _targetObj.GetComponent<Collider>().isTrigger = false;
            _targetObj.GetComponent<Rigidbody>().useGravity = true;
        }
        ThrowObject();
        _targetObj = null;
        isSkillUsing = false;
    }
    private void ThrowObject()
    {
        // 던질때
        if (_targetObj == null)
        {
            return;
        }

        if (Vector3.Distance(_targetObj.position, _endPos) > 1.0f)
        {
            var dir = _endPos - _targetObj.position;
            dir.Normalize();
            _targetObj.GetComponent<Rigidbody>().AddForce(dir * _force, ForceMode.Impulse);
        }
    }




    // 물건을 잡고있을때 
    private void StayObject()
    {
        if (_targetObj == null)
        {
            return;
        }

        _targetObj.GetComponent<Collider>().isTrigger = true;
        _targetObj.GetComponent<Rigidbody>().useGravity = false;
        _realTime += Time.deltaTime;

        // 물체 들어 올렸으때 회전 및 움직임

        _targetObj.position = new Vector3(_targetObj.position.x,
            Mathf.Lerp(_targetObj.position.y, transform.position.y + 0.8f, 0.1f), _targetObj.position.z);

        if (Vector3.Distance(_endPos, _targetObj.position) > 0.03f)
        {
            _targetObj.position = new Vector3(Mathf.Lerp(_targetObj.position.x, _endPos.x, 0.1f),
                _targetObj.position.y, Mathf.Lerp(_targetObj.position.z, _endPos.z, 0.1f));
        }

        float sin = Mathf.Sin(_realTime * Mathf.Deg2Rad);
        float cos = Mathf.Cos(_realTime * Mathf.Deg2Rad);


        _targetObj.rotation = Quaternion.Euler
            (new Vector3(sin * 360.0f * _rotateSpeed, cos * 360.0f * _rotateSpeed, cos * 360.0f * _rotateSpeed));
    }
}
