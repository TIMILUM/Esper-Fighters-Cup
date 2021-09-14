using UnityEngine;


// �����ϰ� ���� ��ų ����
public class SimpleSuperStrength : SkillBase
{

    [SerializeField] private float _Force;
    private Transform _TargetObj;

    private Vector3 _EndPos;
    // ��ų ����
    public override void SkillExecute()
    {
        MousePicking();
    }

    // ��ų�� ��������.
    public override bool EndSkill()
    {

        isSkillUsing = false;
        return true;



    }
    // ������ �̺�Ʈ
    private void ThrowObject()
    {
        if (_TargetObj == null) return;

        var dir = _EndPos - transform.position;
        dir.Normalize();
        _TargetObj.GetComponent<Rigidbody>().AddForce(dir * _Force, ForceMode.Impulse);

    }

    //���콺 ��ŷ
    public void MousePicking()
    {
        //������ ������ �Ÿ��� ����ؼ� ������
        if (Vector3.Distance(transform.position, _EndPos) > _SKillRange)
            return;

        MousePick();

    }

    private void MousePick()
    {

        // ���콺 ��ŷ�϶� 
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hitinfo;

        if (Physics.Raycast(ray, out Hitinfo))
        {

            //��Ʈ���̶� �±׸� �Ǻ�
            if (Hitinfo.transform.tag == "Object" && _SkillKeyCode != SKILLKEYCODE.CONTROL)
            {

                if (_TargetObj == null)
                {
                    // ��ü�� �տ� �ִ��� �Ǻ�
                    if (Physics.Raycast(transform.position, transform.forward, _SKillRange))
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

        // ��ų Ű�ڵ尡 ��Ʈ���϶� 
        if (_SkillKeyCode == SKILLKEYCODE.CONTROL)
        {
            if (Physics.Raycast(transform.position, transform.forward, out Hitinfo, _SKillRange))
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

        //����
        if (_TargetObj != null)
        {
            _TargetObj.GetComponent<Collider>().isTrigger = false;
            _TargetObj.GetComponent<Rigidbody>().useGravity = true;
            _TargetObj.GetComponent<Rigidbody>().freezeRotation = false;
            _TargetObj.transform.parent = null;
            _TargetObj.transform.tag = "Object"; // ��� �ִ� ����
            ThrowObject();

            _TargetObj = null;
        }

    }

    //������ ������ istrigger Ȱ��ȭ �߷°� ��Ȱ��ȭ
    private void GrapObject(RaycastHit Hitinfo)
    {
        _TargetObj = Hitinfo.transform;
        _TargetObj.transform.parent = transform.Find("SkillShotPoint");
        _TargetObj.GetComponent<Collider>().isTrigger = true;
        _TargetObj.GetComponent<Rigidbody>().useGravity = false;
        _TargetObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _TargetObj.GetComponent<Rigidbody>().freezeRotation = true;

        _TargetObj.transform.tag = "GrapObject"; // ��� �ִ� ����
        _TargetObj.localPosition = new Vector3(0.0f, 0.0f, 1.0f);


    }


}
