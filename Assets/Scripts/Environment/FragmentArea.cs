using System.Collections.Generic;
using UnityEngine;

public class FragmentArea : AStaticObject
{
    [SerializeField] private GameObject _frangmentArea;
    [SerializeField] private GameObject _colliderParentObject;
    [SerializeField] private ColliderChecker _collider;
    [SerializeField] private float _fragmentRatio;
    [SerializeField] private List<Actor> _actors = new List<Actor>();


    private bool _isFragmentActive;
    private bool _isNormalActive;
    private bool _isObjectSpawn = false;

    public float Range { get; set; }
    public GameObject FrangmentArea => _frangmentArea;

    protected override void Start()
    {
        base.Start();
        _collider.OnCollision += SetHit;
        Range = transform.localScale.x / 2.0f;
        GetComponent<Rigidbody>().useGravity = false;
    }

    private void Update()
    {
        CreateObject();
    }

    public void FragmentAreaActive()
    {
        _frangmentArea.SetActive(true);
        _isNormalActive = true;
    }

    public void FragmentActive()
    {
        _isFragmentActive = true;
        _frangmentArea.SetActive(false);

    }


    public void StartEvent()
    {
        _colliderParentObject.SetActive(true);

    }

    public void NotFloatObject(Actor castSkill)
    {
        _actors.Add(castSkill);
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

            if (!_isObjectSpawn)
            {
                var randomPosition = Random.insideUnitSphere * Range;
                randomPosition.y = 0.0f;
                _frangmentArea.SetActive(false);
                InGameSkillManager.Instance.CreateSkillObject("Stone", transform.position);
                ParticleManager.Instance.PullParticle("ReverseGravityFiled", transform.position, Quaternion.identity);
                ParticleManager.Instance.PullParticle("ReverseGravityBreak", transform.position, Quaternion.identity);
                ParticleManager.Instance.PullParticle("ReverseGravityFiledAfter", transform.position + new Vector3(0.0f, 0.01f, 0.0f), Quaternion.identity);
                _isObjectSpawn = true;
            }


        }

        if (_isFragmentActive)
        {
            if (!_isObjectSpawn)
            {
                if (!_isObjectSpawn)
                {
                    InGameSkillManager.Instance.CreateSkillObject("Fragment", transform.position);
                    ParticleManager.Instance.PullParticle("ReverseGravityFiled", transform.position, Quaternion.identity);
                    ParticleManager.Instance.PullParticle("ReverseGravityBreak", transform.position, Quaternion.identity);
                    _isObjectSpawn = true;
                }
            }
        }

    }



    public void CancelEvent()
    {
        _isNormalActive = false;
        _isFragmentActive = false;

        _colliderParentObject.SetActive(false);

    }

    public bool GetActive()
    {
        return _isNormalActive;
    }

    public void DirectionLookAt(Vector3 pos)
    {
        var lookDirection = pos - transform.position;

    }

    /// <summary>
    /// 띄운 애들을 넉백을 하기 위한 함수입니다.
    /// </summary>
    /// <param name="buffstruct">띄운다음에 넉백을 적용하기 위해서 buffstruct를 받습니다.</param>
    /// <param name="target">마우스 위치를 말합니다.</param>


    public void ClearFragmentArea()
    {
        if (_isFragmentActive)
        {
            _frangmentArea.SetActive(false);

        }

        _colliderParentObject.SetActive(false);
        _isNormalActive = false;
        _isFragmentActive = false;
        _actors.Clear();
    }

    /// <summary>
    /// 원안에 있는 객체들 한번만 띄우게 만들기위해서 List로 관리 해줬습니다.
    /// </summary>
    public override void SetHit(ObjectBase to)
    {
        if (!_actors.Contains((Actor)to))
        {
            base.SetHit((Actor)to);
            _actors.Add((Actor)to);
        }
    }


}
