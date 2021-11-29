using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EsperFightersCup
{
    public class GrabSkill : SkillObject
    {

        [SerializeField]
        private float _range;


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

        protected override void OnInitializeSkill()
        {
            base.OnInitializeSkill();
            _collider.OnCollision += SetHit;
            var colliderScale = new Vector3(_colliderSize.x, 1.0f, _colliderSize.y);
            _collider.transform.localScale = colliderScale;
        }


        public override void OnPlayerHitEnter(GameObject other)
        {

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

            var dist = Vector3.Distance(Author.transform.position, to.transform.position);
            if (dist < _minDistance)
            {
                _grabTarget = to;
                _minDistance = dist;

                /// 잡기 스킬 시작 위치
                _buffOnCollision[0].ValueVector3[1] = _grabTarget.transform.position;
                _buffOnCollision[0].ValueVector3[0] = transform.position;
            }


        }




        protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
        {
        }

        protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
        {

            _grabTarget = null;

            _colliderParent.SetActive(true);
            _collider.OnCollision += SetHit;

            await UniTask.NextFrame();

            _collider.OnCollision -= SetHit;
            _colliderParent.SetActive(false);


            if (_grabTarget == null)
                return false;

            return true;
        }

        protected override void BeforeFrontDelay()
        {
        }

        protected async override UniTask OnUseAsync()
        {

            var currentTime = 0.0f;
            if (AuthorPlayer.photonView.IsMine)
            {

                var Obj = _grabTarget as Actor;
                AuthorPlayer.Animator.SetBool("Grab", true);

                _buffOnCollision[0].ValueFloat[0] = Author.photonView.ViewID;
                Obj.BuffController.GenerateBuff(_buffOnCollision[0]);
                AuthorPlayer.Animator.SetTrigger("Grab");

                await UniTask.WaitUntil(() =>
                {


                    if (Input.GetMouseButton(0))
                    {
                        AuthorPlayer.Animator.SetTrigger("Throw");
                        return true;
                    }

                    return false;
                });


                await UniTask.WaitUntil(() =>
                {
                    currentTime += Time.deltaTime * 1000.0f;

                    if (currentTime > _ThrowfrontDelayTime)
                    {
                        _buffOnCollision[1].ValueVector3[0] = Vector3.Normalize(GetMousePosition() - _grabTarget.transform.position);
                        Obj.BuffController.GenerateBuff(_buffOnCollision[1]);
                        return true;
                    }
                    return false;
                });

                await UniTask.WaitUntil(() =>
                {
                    if (currentTime > _ThrowendDelayTime)
                    {
                        return true;
                    }
                    return false;
                });
            }

        }

        protected override void BeforeEndDelay()
        {

        }

        protected override void OnCancel()
        {

        }

        protected override void OnRelease()
        {

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


    }
}
