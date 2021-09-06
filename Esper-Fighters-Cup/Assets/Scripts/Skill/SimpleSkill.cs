using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//간단하게 염동력 구현
public class SimpleSkill : SkillBase
{
    [SerializeField] private float _Force;
    [SerializeField] private float _RotateSpeed;
    
    private Transform _TargetObj;
    private Vector3 _EndPos;
    private float RealTime;


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

        if (Vector3.Distance(transform.position, _EndPos) > _SKillRange && _TargetObj == null)
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hitinfo;

        if (Physics.Raycast(ray, out Hitinfo))
        {
            if (Hitinfo.transform.tag == "Object")
            {
                if (_TargetObj == null)
                {
                    _TargetObj = Hitinfo.transform;
                    _TargetObj.transform.GetComponent<Rigidbody>().velocity = Vector3.zero; // 힘 크기를 0으로 시켜줍니다.


                }
            }
            _EndPos = Hitinfo.point;
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
        if (_TargetObj != null)
        {
            _TargetObj.GetComponent<Collider>().isTrigger = false;
            _TargetObj.GetComponent<Rigidbody>().useGravity = true;
        }
        ThrowObject();
        _TargetObj = null;
        isSkillUsing = false;
    }
    private void ThrowObject()
    {
        // 던질때
        if (_TargetObj == null) return;

        var dir = _EndPos - _TargetObj.position;
        dir.Normalize();
        _TargetObj.GetComponent<Rigidbody>().AddForce(dir * _Force, ForceMode.Impulse);
    }




    // 물건을 잡고있을때 
    private void StayObject()
    {
        if (_TargetObj == null) return;

        _TargetObj.GetComponent<Collider>().isTrigger = true;
        _TargetObj.GetComponent<Rigidbody>().useGravity = false;
        RealTime += Time.deltaTime;



        // 물체 들어 올렸으때 회전 및 움직임

        _TargetObj.position = new Vector3 (_TargetObj.position.x ,
            Mathf.Lerp(_TargetObj.position.y, transform.position.y + 0.8f , 0.1f), _TargetObj.position.z);

        if (Vector3.Distance(_EndPos, _TargetObj.position) > 0.03f)
        {
        _TargetObj.position = new Vector3(Mathf.Lerp(_TargetObj.position.x, _EndPos.x, 0.1f),
            _TargetObj.position.y , Mathf.Lerp(_TargetObj.position.z, _EndPos.z, 0.1f));
        }

        float Sin = Mathf.Sin(RealTime * Mathf.Deg2Rad );
        float Cos = Mathf.Cos(RealTime * Mathf.Deg2Rad );


        _TargetObj.rotation =  Quaternion.Euler
            (new Vector3(Sin * 360.0f *_RotateSpeed, Cos  * 360.0f * _RotateSpeed , Cos * 360.0f * _RotateSpeed));

    }
}
