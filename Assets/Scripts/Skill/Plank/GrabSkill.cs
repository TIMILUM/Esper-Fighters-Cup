using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public class GrabSkill : SkillObject
    {

        [SerializeField]
        private float _range;

        [SerializeField]
        private float _grabFrontDelayTime;

        [SerializeField]
        private float _grabEndDelayTime;

        [SerializeField]
        private float _ThrowfrontDelayTime;

        [SerializeField]
        private float _ThrowendDelayTime;


        [SerializeField]
        private ColliderChecker _collider;

        [SerializeField]
        private GameObject _colliderParent;


        [SerializeField]
        private Vector2 _colliderSize = new Vector2(2.0f, 1.0f);

        [SerializeField]
        private float _grabSecond;


        private float _currentSecond;

        private ObjectBase _grabTarget;
        private Vector3 _startPos = Vector3.zero;
        private float _minDistance = float.MaxValue;

        protected override void Start()
        {
            base.Start();
            _collider.OnCollision += SetHit;
            var colliderScale = new Vector3(_colliderSize.x, 0.0f, _colliderSize.y);
            _collider.transform.localScale = colliderScale;

        }

        public override void OnPlayerHitEnter(GameObject other)
        {
            throw new System.NotImplementedException();
        }

        public override void SetHit(ObjectBase to)
        {
            if (to == _grabTarget || _grabTarget is ACharacter) return;

            var charactor = to as ACharacter;


            if (charactor != null)
            {
                _grabTarget = to;
                _startPos = _grabTarget.transform.position;
                return;
            }

            var dist = Vector3.Distance(_player.transform.position, to.transform.position);
            if (dist < _minDistance)
            {
                _grabTarget = to;
                _minDistance = dist;
                _startPos = _grabTarget.transform.position;
            }
        }

        protected override IEnumerator OnCanceled()
        {
            SetState(State.Release);
            yield return null;
        }

        protected override IEnumerator OnEndDelay()
        {
            SetNextState();
            yield break;
        }

        protected override IEnumerator OnFrontDelay()
        {
            while (_currentSecond > 1.0f)
            {
                _currentSecond += Time.deltaTime * _grabSecond;
                _grabTarget.transform.position = Vector3.Lerp(_startPos, _player.transform.position +
                    _player.transform.up * 3.0f, _currentSecond);
                yield return null;
            }

            SetNextState();
            yield break;
        }

        protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerator OnReadyToUse()
        {
            yield return new WaitForSeconds(_grabFrontDelayTime * 0.001f);
            _colliderParent.SetActive(true);
            /// 잡기 애니메이션
            yield return new WaitForSeconds(_grabEndDelayTime * 0.001f);

            _colliderParent.SetActive(false);

            ///물체 없을때 CANCEL
            if (_grabTarget == null)
                SetState(State.Canceled);

            SetNextState();
            yield break;
        }

        protected override IEnumerator OnRelease()
        {
            ApplyMovementSpeed(State.Release);
            Destroy(gameObject);
            yield return null;
        }

        protected override IEnumerator OnUse()
        {

            yield return new WaitForSeconds(_ThrowfrontDelayTime * 0.001f);

            /// 던지기 애니메이션

            yield return new WaitForSeconds(_ThrowendDelayTime * 0.001f);
            SetNextState();
            yield break;

        }
    }
}
