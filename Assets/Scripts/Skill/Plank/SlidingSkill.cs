using System.Collections;
using UnityEngine;


namespace EsperFightersCup
{

    public class SlidingSkill : SkillObject
    {
        [SerializeField]
        private float _slidingRange;
        [SerializeField]
        private float _grabRange;

        [SerializeField]
        private float _slidingFrontDelay;
        [SerializeField]
        private float _slidingEndDelay;

        [SerializeField]
        private float _grapFrontDelay;
        [SerializeField]
        private float _grapEndDelay;


        [SerializeField]
        private float _throwFrontDelay;
        [SerializeField]
        private float _throwEndDelay;



        private GameObject _targetObj = null;
        private float _targetRange = 1.5f;




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
            SetState(State.Release);
            yield return null;
        }

        protected override IEnumerator OnEndDelay()
        {
            SetNextState();
            yield break;
        }


        /// <summary>
        /// 여기서 잡기 기술이 들어갑니다.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator OnFrontDelay()
        {
            var isCanceled = false;


            yield return new WaitUntil(() =>
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    _buffController.ReleaseBuff(BuffObject.Type.Sliding);
                    isCanceled = true;
                }
                if (_buffController.GetBuff(BuffObject.Type.Sliding) == null)
                {
                    isCanceled = true;
                    return isCanceled;
                }

                ///잡기 시킬 사용
                if (Input.GetMouseButtonDown(0))
                {
                    _player.CharacterAnimatorSync.SetTrigger("Grab");

                    /// 박스로 체크 여기서 피봇을 맞춰줘서 뒤에있는 오브젝트가 잡히지 않도록 합니다.
                    var colliders = Physics.OverlapSphere(Author.transform.position, _grabRange);

                    var minDistance = float.MaxValue;
                    foreach (var item in colliders)
                    {



                        if (item.gameObject == Author.gameObject)
                            continue;
                        if (!item.TryGetComponent(out Actor otherActor))
                        {
                            continue;
                        }


                        // 앞뒤 판별합니다.
                        var lookAtDirection = item.transform.position - Author.transform.position;
                        var tempPos = Author.transform.forward;
                        var targetDirctionNormal = lookAtDirection.normalized;
                        float cos = Vector3.Dot(tempPos, targetDirctionNormal);

                        if (cos <= 0)
                            continue;

                        /// 만약 오브젝트가 플레이어일 경우
                        if (item.TryGetComponent(out ACharacter otherChracter))
                        {
                            _targetObj = item.gameObject;
                            break;
                        }
                        /// 거리를 계산해서 가장 가까운 오브젝트를 잡습니다.
                        float distance = Vector3.Distance(Author.transform.position, item.transform.position);
                        if (distance < minDistance)
                        {
                            _targetObj = item.gameObject;
                            minDistance = distance;
                        }
                    }


                    /// 오브젝트가 잡히면 슬라이딩 종료
                    if (_targetObj != null)
                    {
                        _buffController.ReleaseBuff(BuffObject.Type.Sliding);
                        return true;
                    }
                }
                return isCanceled;
            });


            if (isCanceled)
            {
                SetState(State.Canceled);
                yield break;
            }


            var startTime = Time.time;
            var currentTime = Time.time;
            var targetStartPos = _targetObj.transform.position;


            /// 오브젝트가 플레이어 앞으로 이동하는 로직입니다.
            while ((currentTime - startTime) * 1000 <= _grapEndDelay)
            {
                if (_targetObj != null)
                {
                    _targetObj.transform.position = Vector3.Lerp(targetStartPos,
                         Author.transform.position + Author.transform.forward * _targetRange, (currentTime - startTime) * 1000 / _grapEndDelay);
                }

                yield return null;
                currentTime = Time.time;
            }

            SetNextState();
            yield break;
        }




        /// <summary>
        /// 이부분에서 슬라이딩 스킬이 들어갑니다.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator OnReadyToUse()
        {
            if (_buffController.GetBuff(BuffObject.Type.Sliding) != null)
            {
                SetState(State.Canceled);
                yield break;
            }

            _player.CharacterAnimatorSync.SetTrigger("Sliding");
            yield return new WaitForSeconds(_slidingFrontDelay / 1000.0f);

            var mousePos = GetMousePosition();
            ///처음 시작 위치
            _buffOnCollision[0].ValueVector3[0] = Author.transform.position;
            ///목표 위치
            _buffOnCollision[0].ValueVector3[1] = Author.transform.position + (mousePos - Author.transform.position).normalized * _slidingRange;
            ///목표까지 가는 시간
            _buffOnCollision[0].ValueVector3[1].y = _buffOnCollision[0].ValueVector3[0].y;
            ///슬라이드 버프 추가
            _buffController.GenerateBuff(_buffOnCollision[0]);

            //yield return new WaitForSeconds(_slidingEndDelay / 1000.0f);

            SetNextState();
            yield break;
        }

        protected override IEnumerator OnRelease()
        {
            Destroy(gameObject);
            yield return null;
        }

        protected override IEnumerator OnUse()
        {
            var startTime = Time.time;
            var currentTime = Time.time;

            ///플레이어가 오브젝트를 잡고 있는지 판단
            if (_targetObj == null)
            {
                SetNextState();
                yield break;
            }


            _targetObj.GetComponent<Actor>().BuffController.GenerateBuff(_buffOnCollision[1]);



            /// 던지기 선딜레이
            while ((currentTime - startTime) * 1000 <= _throwFrontDelay)
            {
                _targetObj.transform.position = Author.transform.position + Author.transform.forward * _targetRange;
                currentTime = Time.time;
                yield return null;
            }

            _player.CharacterAnimatorSync.SetTrigger("Raise");
            yield return new WaitUntil(() =>
            {
                var mousePos = GetMousePosition();
                _targetObj.transform.position = Author.transform.position + Author.transform.forward * _targetRange;
                if (Input.GetMouseButtonDown(0))
                {
                    _buffOnCollision[2].ValueVector3[0] = (mousePos - Author.transform.position).normalized;
                    _targetObj.GetComponent<Actor>().BuffController.GenerateBuff(_buffOnCollision[2]);
                    _targetObj.GetComponent<Actor>().BuffController.ReleaseBuff(BuffObject.Type.Grab);
                    return true;

                }

                return false;
            });

            startTime = Time.time;
            currentTime = Time.time;

            /// 던지기 후딜레이
            while ((currentTime - startTime) * 1000 <= _throwEndDelay)
            {
                currentTime = Time.time;
                yield return null;
            }


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



        private Vector3 SetStartPos()
        {
            var startPos = GetMousePosition();
            if (Vector3.Distance(startPos, transform.position) > _grabRange)
            {
                return Vector3.positiveInfinity;
            }

            return startPos;
        }



    }
}
