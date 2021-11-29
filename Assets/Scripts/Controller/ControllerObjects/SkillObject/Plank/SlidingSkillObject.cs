using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EsperFightersCup
{
    public class SlidingSkillObject : SkillObject
    {
        private Vector3 _mousePosition;

        protected override void OnInitializeSkill()
        {
            // _buffOnCollision[0] = Slide
            // _buffOnCollision[1] = Grab
            // _buffOnCollision[2] = Knockback
        }

        /// <summary>sad
        /// 이부분에서 슬라이딩 스킬이 들어갑니다.
        /// </summary>
        protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
        {
            _mousePosition = GetMousePosition();
            await UniTask.Yield(cancellationToken: cancellation);
            return true;
        }

        /// <summary>
        /// 여기서 잡기 기술이 들어갑니다.
        /// </summary>
        protected override void BeforeFrontDelay()
        {
        }

        protected override async UniTask OnUseAsync()
        {
            AuthorPlayer.Animator.SetTrigger("Sliding");

            ///처음 시작 위치
            _buffOnCollision[0].ValueVector3[0] = Author.transform.position;
            ///목표 위치
            _buffOnCollision[0].ValueVector3[1] = Author.transform.position + ((_mousePosition - Author.transform.position).normalized * Range);
            ///목표까지 가는 시간
            _buffOnCollision[0].ValueVector3[1].y = _buffOnCollision[0].ValueVector3[0].y;
            ///슬라이드 버프 추가
            BuffController.GenerateBuff(_buffOnCollision[0]);

            await UniTask.Yield();
        }

        protected override void BeforeEndDelay()
        {
            if (Input.GetMouseButton(1))
            {
                Release();
            }
        }

        protected override void OnRelease()
        {
            AuthorPlayer.Animator.SetBool("Cancel", true);
        }

        protected override void OnCancel()
        {
            BuffController.ReleaseBuffsByType(BuffObject.Type.Sliding);
            AuthorPlayer.Animator.SetBool("Cancel", false);
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
