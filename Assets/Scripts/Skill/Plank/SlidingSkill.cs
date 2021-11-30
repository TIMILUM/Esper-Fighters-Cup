using System.Collections;
using UnityEngine;


namespace EsperFightersCup
{

    public class SlidingSkill : SkillObject
    {
        [SerializeField]
        private float _slidingRange;

        [SerializeField]
        private float _slidingFrontDelay;
        [SerializeField]
        private float _slidingEndDelay;



        /// <summary>
        ///
        /// _buffOnCollision[0] = 슬라이드
        /// _buffOnCollision[1] = Grab
        /// _buffOnCollision[2] = Knockback
        ///
        /// </summary>

        public override void OnPlayerHitEnter(GameObject other)
        {

        }

        protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
        {
            throw new System.NotImplementedException();
        }





        protected override IEnumerator OnCanceled()
        {
            yield return new WaitForSeconds(0.03f);
            SyncState(State.Release);
            yield return null;
        }

        protected override IEnumerator OnEndDelay()
        {
            var isCanceled = false;
            var startTime = Time.time;
            var currentTime = Time.time;

            while ((currentTime - startTime) * 1000 <= _slidingEndDelay)
            {
                currentTime = Time.time;
                yield return null;

            }

            while (BuffController.ActiveBuffs.Exists(BuffObject.Type.Sliding))
            {
                if (Input.GetMouseButton(1))
                {
                    BuffController.ReleaseBuffsByType(BuffObject.Type.Sliding);
                    isCanceled = true;
                    break;
                }
                yield return null;

            }

            AuthorPlayer.Animator.SetBool("Cancel", true);

            if (isCanceled)
            {
                SyncState(State.Canceled);
                yield break;
            }

            SetNextState();
        }


        /// <summary>
        /// 여기서 잡기 기술이 들어갑니다.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator OnFrontDelay()
        {
            AuthorPlayer.Animator.SetTrigger("Sliding");
            yield return new WaitForSeconds(_slidingFrontDelay / 1000.0f);


            if (BuffController.ActiveBuffs.Exists(BuffObject.Type.Sliding))
            {
                SyncState(State.Canceled);
                yield break;
            }

            var mousePos = GetMousePosition();
            ///처음 시작 위치
            _buffOnCollision[0].ValueVector3[0] = Author.transform.position;
            ///목표 위치
            _buffOnCollision[0].ValueVector3[1] = Author.transform.position + ((mousePos - Author.transform.position).normalized * _slidingRange);
            ///목표까지 가는 시간
            _buffOnCollision[0].ValueVector3[1].y = _buffOnCollision[0].ValueVector3[0].y;
            ///슬라이드 버프 추가
            BuffController.GenerateBuff(_buffOnCollision[0]);


            SetNextState();
            yield break;
        }

        /// <summary>sad
        /// 이부분에서 슬라이딩 스킬이 들어갑니다.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator OnReadyToUse()
        {

            //yield return new WaitForSeconds(_slidingEndDelay / 1000.0f);

            SetNextState();
            yield break;
        }

        protected override IEnumerator OnRelease()
        {
            AuthorPlayer.Animator.SetBool("Cancel", false);
            Destroy(gameObject);
            yield return null;
        }

        protected override IEnumerator OnUse()
        {
            SetNextState();
            yield break;
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
