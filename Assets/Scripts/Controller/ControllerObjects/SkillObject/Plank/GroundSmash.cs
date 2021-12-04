using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EsperFightersCup
{


    public class GroundSmash : SkillObject
    {

        private Vector2 _currentSize;
        private Vector3 _startPos;
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
            _startPos = new Vector3(Author.transform.position.x, 0.03f, Author.transform.position.z);
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
            InGameSkillManager.Instance.CreateSkillObject("SkillRockObj", _startPos, Author.transform.rotation);
            ParticleManager.Instance.PullParticleSync("GroundSkill", _startPos + Author.transform.forward, Quaternion.identity);
            await UniTask.NextFrame();


        }
    }
}
