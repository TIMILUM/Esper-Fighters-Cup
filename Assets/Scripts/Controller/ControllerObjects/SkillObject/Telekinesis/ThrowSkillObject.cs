using System.Collections;
using EsperFightersCup;
using UnityEngine;

public class ThrowSkillObject : SkillObject
{
    [SerializeField]
    private GameObject _fragmentCasting;
    [SerializeField]
    private GameObject _hitBox;
    [SerializeField]
    private float _range;

    [SerializeField]
    private ColliderChecker _collider;
    [SerializeField]
    private float _frontDelayTime;

    [SerializeField]
    private float _endDelayTime;


    private GameObject _fragmentObj;
    private GameObject _fragmentUI;


    private Vector3 _endMousePos;
    protected override void Start()
    {
        base.Start();
        _range = GetCSVData<float>("Range") * 0.01f;
        _frontDelayTime = FrontDelayMilliseconds;
        _endDelayTime = EndDelayMilliseconds;
        _collider.OnCollision += SetHit;
        GameObjectUtil.ScaleGameObject(_fragmentCasting, new Vector3(_range * 2.0f, 1.0f, _range * 2.0f));
        GameObjectUtil.ScaleGameObject(_hitBox, new Vector3(_range * 2.0f, 1.0f, _range * 2.0f));

    }
    public override void SetHit(ObjectBase to)
    {

        if (_fragmentObj != null)
        {
            if (_fragmentObj.GetComponent<ObjectBase>() == to)
                return;
        }
        base.SetHit(to);
    }


    public override void OnPlayerHitEnter(GameObject other)
    {
        Debug.Log(other.transform.name);
    }

    protected override IEnumerator OnCanceled()
    {
        ApplyMovementSpeed(State.Canceled);
        SetState(State.Release);
        yield return null;
    }

    protected override IEnumerator OnEndDelay()
    {
        ApplyMovementSpeed(State.EndDelay);
        GameObjectUtil.ActiveGameObject(_hitBox);
        GameObjectUtil.TranslateGameObject(_hitBox, _fragmentCasting.transform.position);
        bool isCanceled = false;
        var startTime = Time.time;
        var currentTime = Time.time;

        while ((currentTime - startTime) * 1000 <= _endDelayTime)
        {
            if (Input.GetMouseButtonDown(1))
            {
                isCanceled = true;
                break;
            }
            yield return null;
            currentTime = Time.time;

        }

        if (isCanceled == true)
        {
            SetState(State.Release);
        }

        SetState(State.Use);
    }

    protected override IEnumerator OnReadyToUse()
    {
        var isCanceled = false;

        GameObjectUtil.ActiveGameObject(_fragmentCasting);

        yield return new WaitUntil(() =>
       {
           var mousePos = GetMousePosition();
           GameObjectUtil.TranslateGameObject(_fragmentCasting, mousePos);

           if (Input.GetKeyDown(KeyCode.Mouse1))
           {
               isCanceled = true;
               return isCanceled;
           }
           if (Input.GetKeyUp(KeyCode.LeftShift))
           {
               if (_player.photonView.IsMine)
               {
                   _fragmentUI = InGameSkillManager.Instance.CreateSkillUI("ThrowUI", _fragmentCasting.transform.position);
                   _fragmentCasting.transform.SetParent(null);
                   _fragmentUI.transform.localScale = _fragmentCasting.transform.localScale;
                   _fragmentCasting.transform.SetParent(transform);
                   _fragmentUI.transform.SetParent(GameObject.Find("UiObject").transform);
                   _endMousePos = mousePos;

               }

               return true;
           }
           return isCanceled;
       });



        if (isCanceled)
        {

            SetState(State.Canceled);
            yield break;
        }

        GameObjectUtil.ActiveGameObject(_fragmentCasting, false);
        SetNextState();
    }

    protected override IEnumerator OnFrontDelay()
    {
        ApplyMovementSpeed(State.FrontDelay);
        bool isCanceled = false;
        var startTime = Time.time;
        var currentTime = Time.time;

        //Idle 상태일때 애니메이션 실행
        if (_player.CharacterAnimatorSync.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            _player.CharacterAnimatorSync.SetTrigger("ReverseGravityUnder");
        }

        //하체는 그냥 실행
        _player.CharacterAnimatorSync.SetTrigger("ReverseGravityA");
        while ((currentTime - startTime) * 1000 <= _frontDelayTime)
        {
            if (Input.GetMouseButtonDown(1))
            {
                isCanceled = true;
                break;
            }
            yield return null;
            currentTime = Time.time;
        }

        if (isCanceled == true)
        {

            SetState(State.Release);
        }
        SetState(State.EndDelay);
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


    protected override IEnumerator OnRelease()
    {
        ApplyMovementSpeed(State.Release);
        InGameSkillManager.Instance.DestroySkillObj(_fragmentUI);
        Destroy(gameObject);
        yield break;
    }

    protected override IEnumerator OnUse()
    {
        ApplyMovementSpeed(State.Use);
        _fragmentObj = InGameSkillManager.Instance.CreateSkillObject("Stone", _endMousePos);
        GameObjectUtil.ActiveGameObject(_hitBox, false);
        yield return null;
        SetState(State.Canceled);
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
        throw new System.NotImplementedException();
    }
}
