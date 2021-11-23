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



        private ObjectBase _grabTarget;
        private float _minDistance = float.MaxValue;

        protected override void Start()
        {
            base.Start();
            _collider.OnCollision += SetHit;
            var colliderScale = new Vector3(_colliderSize.x, 1.0f, _colliderSize.y);
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
                /// 잡기 스킬 시작 위치
                _buffOnCollision[0].ValueVector3[1] = _grabTarget.transform.position;
                _buffOnCollision[0].ValueVector3[0] = transform.position;
                return;
            }

            var dist = Vector3.Distance(_player.transform.position, to.transform.position);
            if (dist < _minDistance)
            {
                _grabTarget = to;
                _minDistance = dist;

                /// 잡기 스킬 시작 위치
                _buffOnCollision[0].ValueVector3[1] = _grabTarget.transform.position;
                _buffOnCollision[0].ValueVector3[0] = transform.position;
            }


        }

        protected override IEnumerator OnCanceled()
        {
            SetState(State.Release);
            yield return null;
        }

        protected override IEnumerator OnEndDelay()
        {
            yield return new WaitForSeconds(_ThrowendDelayTime * 0.001f);

            SetNextState();
            yield break;
        }

        protected override IEnumerator OnFrontDelay()
        {
            Actor isBuffController = _grabTarget as Actor;

            if (isBuffController != null)
            {
                isBuffController.BuffController.GenerateBuff(_buffOnCollision[0]);
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
            Actor isBuffController = _grabTarget as Actor;
            if (isBuffController != null)
            {
                isBuffController.BuffController.ReleaseBuff(BuffObject.Type.Grab);
            }


            Destroy(gameObject);
            yield return null;
        }


        protected override IEnumerator OnUse()
        {
            var isCanceled = false;

            yield return new WaitForSeconds(_ThrowfrontDelayTime * 0.001f);


            yield return new WaitUntil(() =>
            {
                if (Input.GetMouseButton(1))
                {
                    isCanceled = true;
                }
                return isCanceled;
            });


            if (isCanceled)
            {
                SetState(State.Canceled);
                yield break;
            }
            SetNextState();
        }
    }
}
