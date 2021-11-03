using System;
using System.Collections;
using System.Collections.Generic;
using EsperFightersCup;
using UnityEngine;

public class ShockWaveSkillObject : SkillObject
{
    [SerializeField]
    private float _range = 3;

    [SerializeField]
    private float _onHitDuration = 0.5f;

    [SerializeField]
    private GameObject[] _firstCasting;

    [SerializeField]
    private GameObject[] _secondCasting;

    [SerializeField]
    [Header("Collider")]
    private Transform _colliderParentTransform;

    [SerializeField]
    private ColliderChecker _collider;

    [SerializeField]
    [Tooltip("[세로, 가로]")]
    private Vector2 _colliderSize = new Vector2(0.5f, 2);

    private Vector3 _direction = Vector3.right;
    private Vector3 _startPos = Vector3.zero;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // 사거리
        _range = GetCSVData<float>("Range");
        // 판정 범위 가로 세로
        _colliderSize = new Vector2(GetCSVData<float>("ShapeData_1"), GetCSVData<float>("ShapeData_2"));
        GameObjectUtil.ScaleGameObjects(_firstCasting, new Vector3(_range, 1, _range));

        var colliderScale = new Vector3(_colliderSize.x, 1, _colliderSize.y);
        GameObjectUtil.ScaleGameObjects(_secondCasting, colliderScale);
        _colliderParentTransform.localScale = colliderScale;
        _collider.OnCollision += SetHit;
    }

    public override void SetHit(ObjectBase to)
    {
        _buffOnCollision[0].ValueVector3[0] = _direction;
        base.SetHit(to);
    }

    protected override IEnumerator OnReadyToUse()
    {
        var isCanceled = false;
        Vector3 endPos;

        GameObjectUtil.ActiveGameObjects(_firstCasting);

        yield return new WaitUntil(() =>
        {
            // 우클릭 시 취소
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                isCanceled = true;
            }

            // 시작점 설정
            if (Input.GetMouseButtonDown(0))
            {
                _startPos = SetStartPos();
                if (float.IsPositiveInfinity(_startPos.x))
                {
                    isCanceled = true;
                }

                GameObjectUtil.ActiveGameObjects(_firstCasting, false);
                GameObjectUtil.ActiveGameObjects(_secondCasting);
            }
            // 끝점 설정
            else if (Input.GetMouseButton(0) && _startPos != Vector3.positiveInfinity)
            {
                endPos = GetMousePosition();
                _direction = Vector3.Normalize(endPos - _startPos);
                GameObjectUtil.TranslateGameObjects(_secondCasting, _startPos);
                GameObjectUtil.RotateGameObjects(_secondCasting, Quaternion.LookRotation(_direction));
            }
            // 판정 범위 최종 계산
            else if (Input.GetMouseButtonUp(0))
            {
                //충격파 애니메이션
                _player.CharacterAnimatorSync.SetTrigger("ShockWaveSkill");
                ParticleManager.Instance.PullParticle("ShockWave", _startPos - (_direction * 2), Quaternion.LookRotation(_direction));
                return true;
            }

            return isCanceled;
        });

        if (isCanceled)
        {
            SetState(State.Canceled);
            yield break;
        }

        GameObjectUtil.ActiveGameObjects(_firstCasting, false);
        GameObjectUtil.ActiveGameObjects(_secondCasting, false);
        SetNextState();
    }

    protected override IEnumerator OnFrontDelay()
    {
        ApplyMovementSpeed(State.FrontDelay);
        yield return new WaitForSeconds(FrontDelayMilliseconds / 1000.0f);
        SetNextState();
        yield break;
    }

    protected override IEnumerator OnUse()
    {
        ApplyMovementSpeed(State.Use);
        _colliderParentTransform.SetPositionAndRotation(_startPos, Quaternion.LookRotation(_direction));
        _colliderParentTransform.gameObject.SetActive(true);
        var startTime = DateTime.Now;
        var nowTime = DateTime.Now;
        _colliderParentTransform.transform.position = _startPos;
        yield return new WaitUntil(() => WaitPhysicsUpdate());
        nowTime = DateTime.Now;

        _colliderParentTransform.gameObject.SetActive(false);
        SetNextState();
    }

    protected override IEnumerator OnEndDelay()
    {
        ApplyMovementSpeed(State.EndDelay);
        yield return new WaitForSeconds(EndDelayMilliseconds / 1000.0f);
        SetNextState();
        yield break;
    }

    protected override IEnumerator OnCanceled()
    {
        ApplyMovementSpeed(State.Canceled);
        SetState(State.Release);
        yield return null;
    }

    protected override IEnumerator OnRelease()
    {
        ApplyMovementSpeed(State.Release);
        Destroy(gameObject);
        yield return null;
    }

    private Vector3 SetStartPos()
    {
        var startPos = GetMousePosition();
        if (Vector3.Distance(startPos, transform.position) > _range)
        {
            return Vector3.positiveInfinity;
        }

        return startPos;
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

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
    }
}
