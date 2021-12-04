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
        private Vector2 _colliderSize = new Vector2(5.0f, 10.0f);


        [SerializeField]
        private int _animationDelay;



        private ObjectBase _grabTarget;
        private float _minDistance = float.MaxValue;

        protected override void OnInitializeSkill()
        {

            base.OnInitializeSkill();


            var colliderScale = new Vector3(_colliderSize.x, 1.0f, _colliderSize.y);
            _collider.transform.localScale = colliderScale;
        }


        public override void OnPlayerHitEnter(GameObject other)
        {

        }

        public override void SetHit(ObjectBase to)
        {

            if (to == _grabTarget || to == null) return;


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
            base.OnHit(this, to, _buffOnCollision.ToArray());
        }




        protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
        {
        }

        protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
        {

            if (AuthorPlayer.photonView.IsMine)
                AuthorPlayer.Animator.SetTrigger("Grab");

            _grabTarget = null;
            _minDistance = float.MaxValue;

            _colliderParent.SetActive(true);
            _collider.OnCollision += SetHit;

            await UniTask.NextFrame();

            _collider.OnCollision -= SetHit;
            _colliderParent.SetActive(false);



            if (_grabTarget == null)
            {
                AuthorPlayer.Animator.SetBool("GrabCancel", true);
                await UniTask.Delay(_animationDelay, cancellationToken : cancellation);
                AuthorPlayer.Animator.SetBool("GrabCancel", false);

                return false;
            }

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

                _buffOnCollision[0].ValueFloat[0] = Author.photonView.ViewID;
                Obj.BuffController.GenerateBuff(_buffOnCollision[0]);

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
                        _buffOnCollision[1].ValueFloat[3] = Author.photonView.ViewID;
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
            AuthorPlayer.Animator.SetBool("Grab", false);
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
