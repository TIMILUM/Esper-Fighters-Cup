using System.Collections.Generic;
using UnityEngine;

public class FragmentArea : AStaticObject
{
    [SerializeField]
    private GameObject _nomalArea;
    [SerializeField]
    private GameObject _frangmentArea;
    [SerializeField]
    private GameObject _colliderParentObject;
    [SerializeField]
    private ColliderChecker _collider;
    [SerializeField]
    private float _fragmentRatio;
    [SerializeField]
    private GameObject _direction;

    [SerializeField, Header("단위 : Millisecond")]
    private float _spawnStoneTime;
    [SerializeField, Header("단위 : Millisecond")]
    private float _spawnFragmentTime;


    private float _currentStoneTime;
    private float _currentfragmentTime;

    private List<Actor> _actors = new List<Actor>();


    public GameObject FrangmentArea => _frangmentArea;
    public float Range { get; set; }

    private bool _isFragmentActive;
    private bool _isNormalActive;




    private new void Start()
    {

        // range랑 크기랑 똑같기 때문에 임의의 좌표로 초기화 해줬습니다.
        // 크기는 지름을 나타내기 때문에 2를 나눠줍니다.
        Range = _nomalArea.transform.localScale.x * 0.5f;
        _collider.OnCollision += SetHit;
    }

    private void Update()
    {
        CreateObject();
    }

    public void FragmentAreaActive()
    {
        _frangmentArea.SetActive(true);
        _nomalArea.SetActive(false);
        _isNormalActive = true;
    }

    public void FragmentActive()
    {
        _isFragmentActive = true;
        _frangmentArea.SetActive(false);
        _nomalArea.SetActive(true);
    }


    public void StartEvent()
    {
        _colliderParentObject.SetActive(true);
        _direction.SetActive(true);
    }

    /// <summary>
    /// 돌덩이 파편 만들기위한 함수입니다.
    /// </summary>
    private void CreateObject()
    {

        if (!_colliderParentObject.activeInHierarchy)
        {
            return;
        }

        if (_isNormalActive)
        {
            _currentStoneTime += Time.deltaTime * 1000;
            if (_currentStoneTime > _spawnStoneTime)
            {
                var randomPosition = Random.insideUnitSphere * Range;
                randomPosition.y = 0.0f;
                InGameSkillManager.Instance.CreateSkillObject("Stone", transform.position + randomPosition);
                _currentStoneTime = 0.0f;
            }
        }

        if (_isFragmentActive)
        {
            _currentfragmentTime += Time.deltaTime * 1000;
            if (_currentfragmentTime > _spawnStoneTime)
            {
                var randomPosition = Random.insideUnitSphere * Range;
                randomPosition.y = 0.0f;
                InGameSkillManager.Instance.CreateSkillObject("Fragment", transform.position + randomPosition);
                _currentfragmentTime = 0.0f;
            }
        }
    }



    public void CancelEvent()
    {
        _isNormalActive = false;
        _isFragmentActive = false;
        _direction.SetActive(false);
        _colliderParentObject.SetActive(false);

    }

    public bool GetActive()
    {
        return _frangmentArea.activeInHierarchy;
    }

    public void DirectionLookAt(Vector3 pos)
    {
        var lookDirection = pos - transform.position;
        _direction.transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    /// <summary>
    /// 띄운 애들을 넉백을 하기 위한 함수입니다.
    /// </summary>
    /// <param name="buffstruct">띄운다음에 넉백을 적용하기 위해서 buffstruct를 받습니다.</param>
    /// <param name="target">마우스 위치를 말합니다.</param>
    public void KnockBackObject(BuffObject.BuffStruct buffstruct, Vector3 target)
    {
        _colliderParentObject.SetActive(false);
        foreach (var actor in _actors)
        {
            if (actor == null)
            {
                continue;
            }

            if (actor.BuffController.GetBuff(BuffObject.Type.Raise) == null)
            {
                continue;
            }

            actor.BuffController.ReleaseBuff(BuffObject.Type.Raise);
            var direction = target - _frangmentArea.transform.position;
            buffstruct.ValueVector3[0] = direction.normalized;
            actor.BuffController.GenerateBuff(buffstruct);
        }
        if (_isFragmentActive)
        {
            _frangmentArea.SetActive(false);
            _nomalArea.SetActive(false);
        }

        _isNormalActive = false;
        _isFragmentActive = false;
        _direction.SetActive(false);
        _actors.Clear();
    }


    //원안에 있는 객체들 한번만 띄우게 만들기위해서 List로 관리 해줬습니다.
    protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
        base.OnHit(from, to, appendBuff);
    }

    public override void SetHit(ObjectBase to)
    {
        if (!_actors.Contains((Actor)to))
        {
            base.SetHit((Actor)to);
            _actors.Add((Actor)to);
        }
    }


}
