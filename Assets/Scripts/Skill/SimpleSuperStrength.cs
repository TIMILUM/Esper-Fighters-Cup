using UnityEngine;


// 간단하게 괴력 스킬 구현
public class SimpleSuperStrength : SkillBase
{

    [SerializeField] private float _Force;
    private Transform _TargetObj;

    private Vector3 _EndPos;
    // 스킬 적용
    public override void SkillExecute()
    {
        MousePicking();
    }

    // 스킬이 끝날을때.
    public override bool EndSkill()
    {

        IsSkillUsing = false;
        return true;



    }
    // 던지기 이벤트
    private void ThrowObject()
    {
        if (_TargetObj == null) return;

        var dir = _EndPos - transform.position;
        dir.Normalize();
        _TargetObj.GetComponent<Rigidbody>().AddForce(dir * _Force, ForceMode.Impulse);

    }

    //마우스 피킹
    public void MousePicking()
    {
        //물건을 잡을때 거리를 계산해서 던지기
        if (Vector3.Distance(transform.position, _EndPos) > _sKillRange)
            return;

        MousePick();

    }

    private void MousePick()
    {

        // 마우스 피킹일때 
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hitinfo;

        if (Physics.Raycast(ray, out Hitinfo))
        {

            //컨트롤이랑 태그를 판별
            if (Hitinfo.transform.tag == "Object" && _skillKeyCode != SKILLKEYCODE.CONTROL)
            {

                if (_TargetObj == null)
                {
                    // 객체가 앞에 있는지 판별
                    if (Physics.Raycast(transform.position, transform.forward, _sKillRange))
                    {
                        Debug.Log(Hitinfo.transform.name);
                        if (Hitinfo.transform.tag != "Object")
                            return;
                    }
                    else
                    {
                        return;
                    }

                    if (_TargetObj == null)
                    {
                        GrapObject(Hitinfo);
                        return;
                    }
                }

            }

            _EndPos = Hitinfo.point;
        }

        // 스킬 키코드가 컨트롤일때 
        if (_skillKeyCode == SKILLKEYCODE.CONTROL)
        {
            if (Physics.Raycast(transform.position, transform.forward, out Hitinfo, _sKillRange))
            {
                if (Hitinfo.transform.tag == "Object")
                {
                    if (_TargetObj == null)
                    {
                        GrapObject(Hitinfo);
                        return;
                    }
                }
            }
        }
        ResetObject();
    }




    private void ResetObject()
    {

        //리셋
        if (_TargetObj != null)
        {
            _TargetObj.GetComponent<Collider>().isTrigger = false;
            _TargetObj.GetComponent<Rigidbody>().useGravity = true;
            _TargetObj.GetComponent<Rigidbody>().freezeRotation = false;
            _TargetObj.transform.parent = null;
            _TargetObj.transform.tag = "Object"; // 잡고 있는 상태
            ThrowObject();

            _TargetObj = null;
        }

    }

    //물건을 잡을때 istrigger 활성화 중력값 비활성화
    private void GrapObject(RaycastHit Hitinfo)
    {
        _TargetObj = Hitinfo.transform;
        _TargetObj.transform.parent = transform.Find("SkillShotPoint");
        _TargetObj.GetComponent<Collider>().isTrigger = true;
        _TargetObj.GetComponent<Rigidbody>().useGravity = false;
        _TargetObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _TargetObj.GetComponent<Rigidbody>().freezeRotation = true;

        _TargetObj.transform.tag = "GrapObject"; // 잡고 있는 상태
        _TargetObj.localPosition = new Vector3(0.0f, 0.0f, 1.0f);


    }


}
