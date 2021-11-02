using System.Collections;
using UnityEngine;

public class ThrowSkillObject : SkillObject
{
    [SerializeField]
    private GameObject _casting;
    [SerializeField]
    private GameObject _fragmentCasting;
    [SerializeField]
    private float _range;
    [SerializeField]
    private float _fragmentAreaRange;

    [SerializeField]
    private float _frontDelayTime;

    [SerializeField]
    private float _EndDelayTime;



    protected override void Start()
    {
        base.Start();
        _range = GetCSVData<float>("Range");
        _frontDelayTime = FrontDelayMilliseconds;
        _EndDelayTime = EndDelayMilliseconds;
        ScaleGameObjects(_casting, new Vector3(_range * 2.0f, 1.0f, _range * 2.0f));
        ScaleGameObjects(_fragmentCasting, new Vector3(_fragmentAreaRange * 2.0f, 1.0f, _fragmentAreaRange * 2.0f));

    }
    public override void SetHit(ObjectBase to)
    {
        base.SetHit(to);
    }


    public override void OnPlayerHitEnter(GameObject other)
    {
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
        bool isCanceled = false;
        var startTime = Time.time;
        var currentTime = Time.time;

        while ((currentTime - startTime) * 1000 <= _EndDelayTime)
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
            InGameSkillManager.Instance.FragmentAllDestroy();
            SetState(State.Release);
        }
        SetState(State.Use);
    }

    protected override IEnumerator OnReadyToUse()
    {
        var isCanceled = false;
        ActiveGameObjects(_casting);
        ActiveGameObjects(_fragmentCasting);

        yield return new WaitUntil(() =>
       {
           var mousePos = GetMousePosition();
           TranslateGameObjects(_fragmentCasting, mousePos);

           if (Input.GetKeyDown(KeyCode.Mouse1))
           {
               isCanceled = true;
               return isCanceled;
           }
           if (Input.GetKeyUp(KeyCode.LeftShift))
           {
               InGameSkillManager.Instance.FragmentAllActive(transform.position, _range, _player.photonView.ViewID);
               ActiveGameObjects(_fragmentCasting, false);
               ActiveGameObjects(_casting, false);
               if (SetPosCompare(mousePos))
               {
                   InGameSkillManager.Instance.AddFragmentArea(_fragmentCasting.transform, _fragmentAreaRange, _player.photonView.ViewID);
                   InGameSkillManager.Instance.FragmentAllActive(GetMousePosition(), _range, _player.photonView.ViewID);
               }
               else
               {
                   isCanceled = true;
                   return isCanceled;
               }

               return true;
           }
           return isCanceled;
       });


        if (isCanceled)
        {
            InGameSkillManager.Instance.FragmentAllDestroy();
            SetState(State.Canceled);
            yield break;
        }

        ActiveGameObjects(_fragmentCasting, false);
        ActiveGameObjects(_casting, false);
        SetNextState();
    }

    protected override IEnumerator OnFrontDelay()
    {
        ApplyMovementSpeed(State.FrontDelay);
        bool isCanceled = false;
        var startTime = Time.time;
        var currentTime = Time.time;
        if (_player.CharacterAnimatorSync.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            _player.CharacterAnimatorSync.SetTrigger("ReverseGravityUnder");
        }
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
            InGameSkillManager.Instance.FragmentAllDestroy();
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
        InGameSkillManager.Instance.FragmentAreaClear();
        Destroy(gameObject);
        yield break;
    }

    protected override IEnumerator OnUse()
    {
        ApplyMovementSpeed(State.Use);
        bool isCanceled = false;

        InGameSkillManager.Instance.FragmentEventStart();


        if (isCanceled)
        {
            InGameSkillManager.Instance.CancelFragment();
            SetState(State.Canceled);
        }
        yield return null;
        SetState(State.Canceled);
    }


    private bool SetPosCompare(Vector3 pos)
    {
        var startPos = GetMousePosition();
        if (Vector3.Distance(pos, transform.position) > _range)
        {
            return false;
        }

        return true;
    }

    private void ActiveGameObjects(GameObject gameObjects, bool isActive = true)
    {
        gameObjects.SetActive(isActive);
    }

    private void RotateGameObjects(GameObject gameObjects, Quaternion rotation)
    {

        gameObjects.transform.rotation = rotation;

    }

    private void ScaleGameObjects(GameObject gameObjects, Vector3 scale)
    {
        gameObjects.transform.localScale = scale;
    }


    private void TranslateGameObjects(GameObject gameObjects, Vector3 position)
    {
        gameObjects.transform.position = position;
    }


    protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
        throw new System.NotImplementedException();
    }
}
