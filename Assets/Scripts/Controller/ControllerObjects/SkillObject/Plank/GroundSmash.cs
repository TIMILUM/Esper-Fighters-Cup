using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EsperFightersCup
{


    public class GroundSmash : SkillObject
    {

        private Vector2 _currentSize;
        private float _currentRange;


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
            _buffOnCollision[1].Duration = 1.0f;
            _buffOnCollision[0].Damage = Damage;
        }

        protected override void BeforeEndDelay()
        {
            var CreatePos = new Vector3(Author.transform.position.x, 0.03f, Author.transform.position.z);
            InGameSkillManager.Instance.CreateSkillObject("SkillRockObj", CreatePos, Author.transform.rotation);
            ParticleManager.Instance.PullParticle("GroundSkill", CreatePos + Author.transform.forward, Quaternion.identity);

        }

        protected override void BeforeFrontDelay()
        {
            GameUIManager.Instance.PlaySync(Author, "GroundSmash_Range", transform.position, _currentSize, Author.transform.rotation.eulerAngles.y, _uIDuration);
            AuthorPlayer.Animator.SetTrigger("GroundSkill");
        }

        protected override void OnCancel()
        {

        }



        /// <summary>
        ///  이부분 SetHit가 안 먹혀서 OnPlayerHit에서 넣어줬습니다.
        /// </summary>
        /// <param name="other"></param>
        public override void OnPlayerHitEnter(GameObject other)
        {
            var to = other.GetComponent<Actor>();

            if (to == null) return;

            SetHit(to);
        }



        protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
        {
            await UniTask.NextFrame();
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


        }
    }
}
