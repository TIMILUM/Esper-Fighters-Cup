
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EsperFightersCup
{
    public class SlidingSkillObject : SkillObject
    {
        private Vector3 _mousePosition;
        [SerializeField, Tooltip("Range의 값")]
        private float _dummyRange;

        protected override void OnInitializeSkill()
        {
            base.OnInitializeSkill();
            // _buffOnCollision[0] = Slide
            // _buffOnCollision[1] = Grab
            // _buffOnCollision[2] = Knockback
        }

        /// <summary>sad
        /// 이부분에서 슬라이딩 스킬이 들어갑니다.
        /// </summary>
        protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
        {
            if (Range > 0)
            {
                _dummyRange = Range;
            }

            _mousePosition = GetMousePosition();
            await UniTask.NextFrame();
            return true;
        }

        protected override void BeforeFrontDelay()
        {
        }

        protected override async UniTask OnUseAsync()
        {
            ///처음 시작 위치
            _buffOnCollision[0].ValueVector3[0] = Author.transform.position;
            ///목표 위치
            _buffOnCollision[0].ValueVector3[1] = Author.transform.position + ((_mousePosition - Author.transform.position).normalized * _dummyRange);
            ///목표까지 가는 시간
            _buffOnCollision[0].ValueVector3[1].y = _buffOnCollision[0].ValueVector3[0].y;
            ///슬라이드 버프 추가
            BuffController.GenerateBuff(_buffOnCollision[0]);

            AuthorPlayer.Animator.SetTrigger("Sliding");
            SfxManager.Instance.PlaySFXSync("Slide", Author.transform.position);
            ParticleManager.Instance.PullParticleAttachedSync("Plank_Sliding_Sonic_Boom", 2);


            await UniTask.Yield();
        }

        protected override void BeforeEndDelay()
        {
        }

        protected override void OnRelease()
        {

        }

        protected override void OnCancel()
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
