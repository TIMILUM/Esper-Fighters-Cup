using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using EsperFightersCup.UI.InGame.Skill;
using UnityEngine;

namespace EsperFightersCup
{


    public class GroundSmash : SkillObject
    {

        private Vector2 _currentSize;
        private float _currentRange;
        [SerializeField] private float _stunDuration;


        [SerializeField] private ColliderChecker _collider;
        [SerializeField] private float _uIDuration;





        protected override void OnInitializeSkill()
        {

            base.OnInitializeSkill();

            _currentRange = Range;
            _currentSize = Size;

            if (Range == 0)
                _currentRange = 5.0f;

            if (Size == Vector2.zero)
                _currentSize = new Vector2(500.0f, 500.0f);


            var colliderScale = new Vector3(_currentRange, 1, _currentRange);
            transform.localScale = colliderScale;
            _buffOnCollision[0].Damage = Damage;
            _buffOnCollision[1].Duration = _stunDuration;
        }

        protected override void BeforeEndDelay()
        {
            var CreatePos = new Vector3(Author.transform.position.x, 0.03f, Author.transform.position.z);
            InGameSkillManager.Instance.CreateSkillObject("SkillRockObj", CreatePos, Author.transform.rotation);
            ParticleManager.Instance.PullParticle("GroundSkill", CreatePos + Author.transform.forward, Quaternion.identity);

        }

        protected override void BeforeFrontDelay()
        {
            GameUIManager.Instance.Play("Skill_Ground_Smash", transform.position, Author.transform.rotation.eulerAngles.y, _currentSize, _uIDuration);
            AuthorPlayer.Animator.SetTrigger("GroundSkill");
        }

        protected override void OnCancel()
        {

        }

        protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
        {
            _collider.OnCollision += SetHit;
            await UniTask.Yield();
            return true;
        }
        public override void SetHit(ObjectBase to)
        {
            base.SetHit(to);
        }



        protected override void OnRelease()
        {

        }

        protected override async UniTask OnUseAsync()
        {
            await UniTask.NextFrame();
            _collider.OnCollision -= SetHit;
        }
    }
}
