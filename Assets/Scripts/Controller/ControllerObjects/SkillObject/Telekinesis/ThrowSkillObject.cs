using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSkillObject : SkillObject
{
    [SerializeField]
    private float _range = 10;
    [SerializeField]
    private GameObject _firstCasting;
    [SerializeField]
    private GameObject _secondCasting;
    [SerializeField]
    private int _maxStoneCount = 5;
    [SerializeField]
    private int _maxFragmentCount = 5;
    [SerializeField, Tooltip("단위 밀리세컨드")]
    private float _stoneSpawnTime = 5000;

    [SerializeField, Tooltip("단위 밀리세컨드")]
    private float _fragmentSpawnTime = 5000;

    [SerializeField]
    private float _stoneSpwanArea;
    [SerializeField]
    private float _onHitDuration = 0.5f;


    private int _currentStoneCount;
    private int _currentFragmentCount;
    private float _startTime;

    private Vector3 _startPos;

    private List<GameObject> _skillobj = new List<GameObject>();

    private Vector3 _direction = Vector3.right;



    [SerializeField]
    [Header("Collider")]
    private Transform _colliderParentTransform;

    [SerializeField]
    private ColliderChecker _collider;



    protected override void Start()
    {
        base.Start();



        ScaleGameObjects(_firstCasting, Vector3.one * _range);
        ScaleGameObjects(_secondCasting, Vector3.one * _range);
        _colliderParentTransform.localScale = Vector3.one * _range;
        _collider.OnCollision += SetHit;

        _range = _range / 2.0f;

    }
    public override void SetHit(ObjectBase to)
    {
        _buffOnCollision[0].ValueVector3[0] = _direction;
        base.SetHit(to);
    }


    public override void OnPlayerHitEnter(GameObject other)
    {
    }

    protected override IEnumerator OnCanceled()
    {
        Debug.Log("스킬 종료");
        SetState(State.Release);
        yield return null;
    }

    protected override IEnumerator OnEndDelay()
    {
        Debug.Log("스킬 종료");
        SetState(State.Release);
        yield return null;
    }





    protected override IEnumerator OnFrontDelay()
    {




        bool isCanceled = false;

        yield return new WaitUntil(() =>
        {
            float CurrentTime = (Time.time - _startTime) * 1000.0f;

            float TempStoneTime = (_stoneSpawnTime / _maxStoneCount) * _currentStoneCount;
            float TempFragmentTime = (_fragmentSpawnTime / _maxFragmentCount) * _currentFragmentCount;

            if (TempStoneTime < CurrentTime && _stoneSpawnTime > CurrentTime)
            {
                _skillobj.Add(InGameSkillManager.Instance.CreateSkillObject("Stone", _startPos + new Vector3(
                    Random.Range(-_range, _range), 1.0f, Random.Range(-_range, _range))));
                _currentStoneCount++;
            }


            if (TempFragmentTime < CurrentTime && _fragmentSpawnTime > CurrentTime)
            {
                _skillobj.Add(InGameSkillManager.Instance.CreateSkillObject("Fragment", _startPos + new Vector3
                    (Random.Range(-_range, _range), 1.0f, Random.Range(-_range, _range))));
                _currentFragmentCount++;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isCanceled = true;
                _direction = GetMousePosition() - _startPos;
            }
            if (Input.GetMouseButton(1))
                SetState(State.Release);


            return isCanceled;
        });

        if (isCanceled)
            SetNextState();


        yield break;
    }


    protected override IEnumerator OnReadyToUse()
    {
        bool isCanceled = false;

        ActiveGameObjects(_firstCasting);
        ActiveGameObjects(_secondCasting);
        InGameSkillManager.Instance.ScrapingAllSetActive();

        yield return new WaitUntil(() =>
        {

            var MousePos = GetMousePosition();
            TranslateGameObjects(_secondCasting, GetMousePosition());

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                ActiveGameObjects(_firstCasting, false);
                ActiveGameObjects(_secondCasting, false);


                if (Input.GetMouseButton(0) && SkillRangeCheck(MousePos))
                {
                    if (!InGameSkillManager.Instance.ScrapingCampare(MousePos))
                    {
                        SetState(State.Release);
                        return isCanceled;
                    }
                    else
                    {
                        InGameSkillManager.Instance.AddScraping(_secondCasting.transform, _range);
                        _startPos = MousePos;
                        _startTime = Time.time;
                        isCanceled = true;
                    }
                }
                else
                {
                    SetState(State.Release);
                    return isCanceled;

                }
            }
            return isCanceled;
        });


        if (isCanceled)
            SetNextState();

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
        InGameSkillManager.Instance.ScrapingAllSetActive(false);
        Destroy(gameObject);
        yield return null;
    }

    protected override IEnumerator OnUse()
    {
        _colliderParentTransform.SetPositionAndRotation(_startPos, Quaternion.LookRotation(_direction));
        _colliderParentTransform.gameObject.SetActive(true);
        var startTime = System.DateTime.Now;
        var nowTime = System.DateTime.Now;
        while ((nowTime - startTime).TotalMilliseconds <= _onHitDuration * 1000)
        {
            _colliderParentTransform.transform.position = _startPos;
            yield return null;
            nowTime = System.DateTime.Now;
        }

        _colliderParentTransform.gameObject.SetActive(false);
        SetNextState();
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

    private bool SkillRangeCheck(Vector3 Target)
    {
        if (Vector3.Distance(_firstCasting.transform.position, Target) < _range)
            return true;
        return false;


    }
}
