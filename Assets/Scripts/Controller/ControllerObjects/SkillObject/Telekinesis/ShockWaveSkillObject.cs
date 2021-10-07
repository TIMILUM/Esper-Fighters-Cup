using System;
using System.Collections;
using System.Collections.Generic;
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
        ScaleGameObjects(_firstCasting, new Vector3(_range, 1, _range));

        var colliderScale = new Vector3(_colliderSize.y, 1, _colliderSize.x);
        ScaleGameObjects(_secondCasting, colliderScale);
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

        ActiveGameObjects(_firstCasting);

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

                ActiveGameObjects(_firstCasting, false);
                ActiveGameObjects(_secondCasting);
            }
            // 끝점 설정
            else if (Input.GetMouseButton(0) && _startPos != Vector3.positiveInfinity)
            {
                endPos = GetMousePosition();
                _direction = Vector3.Normalize(endPos - _startPos);
                TranslateGameObjects(_secondCasting, _startPos);
                RotateGameObjects(_secondCasting, Quaternion.LookRotation(_direction));
            }
            // 판정 범위 최종 계산
            else if (Input.GetMouseButtonUp(0))
            {
                return true;
            }

            return isCanceled;
        });

        if (isCanceled)
        {
            SetState(State.Canceled);
            yield break;
        }

        ActiveGameObjects(_firstCasting, false);
        ActiveGameObjects(_secondCasting, false);
        SetNextState();
    }

    protected override IEnumerator OnFrontDelay()
    {
        SetNextState();
        yield break;
    }

    protected override IEnumerator OnUse()
    {
        _colliderParentTransform.SetPositionAndRotation(_startPos, Quaternion.LookRotation(_direction));
        _colliderParentTransform.gameObject.SetActive(true);
        var startTime = DateTime.Now;
        var nowTime = DateTime.Now;
        while ((nowTime - startTime).TotalMilliseconds <= _onHitDuration * 1000)
        {
            _colliderParentTransform.transform.position = _startPos;
            yield return null;
            nowTime = DateTime.Now;
        }

        _colliderParentTransform.gameObject.SetActive(false);
        SetNextState();
    }

    protected override IEnumerator OnEndDelay()
    {
        SetNextState();
        yield break;
    }

    protected override IEnumerator OnCanceled()
    {
        SetState(State.Release);
        yield return null;
    }

    protected override IEnumerator OnRelease()
    {
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

    private void ActiveGameObjects(IEnumerable<GameObject> gameObjects, bool isActive = true)
    {
        foreach (var gameObj in gameObjects)
        {
            gameObj.SetActive(isActive);
        }
    }

    private void RotateGameObjects(IEnumerable<GameObject> gameObjects, Quaternion rotation)
    {
        foreach (var gameObj in gameObjects)
        {
            gameObj.transform.rotation = rotation;
        }
    }

    private void ScaleGameObjects(IEnumerable<GameObject> gameObjects, Vector3 scale)
    {
        foreach (var gameObj in gameObjects)
        {
            gameObj.transform.localScale = scale;
        }
    }

    private void TranslateGameObjects(IEnumerable<GameObject> gameObjects, Vector3 position)
    {
        foreach (var gameObj in gameObjects)
        {
            gameObj.transform.position = position;
        }
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
    }
}
