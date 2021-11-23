using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public class DropSkillObject : SkillObject
    {


        [SerializeField]
        private float _range;
        private float _secondrange;

        [SerializeField]
        private float _frontDelayTime;

        [SerializeField]
        private float _endDelayTime;

        [SerializeField]
        private GameObject[] _secondCasting;

        private Vector3 _endMousePoint;

        private static readonly Dictionary<int, float> s_dropObjectPercentageData = new Dictionary<int, float>();
        private int _targetId = 0;

        protected override void Start()
        {

            base.Start();
            SetDropObjectCSVData();
            _targetId = GetRandomDropObjectID();
            _range = GetCSVData<float>("Range") * 0.001f;
            _secondrange = 0.2f;
            _frontDelayTime = FrontDelayMilliseconds;
            _endDelayTime = EndDelayMilliseconds;


            ScaleGameObjects(_secondCasting, new Vector3(_secondrange * 2.0f, 1.0f, _secondrange * 2.0f));

        }

        public override void OnPlayerHitEnter(GameObject other)
        {

        }

        protected override IEnumerator OnCanceled()
        {
            ApplyMovementSpeed(State.Canceled);
            SetState(State.Release);
            yield return null;
        }

        protected override IEnumerator OnEndDelay()
        {
            ApplyMovementSpeed(State.EndDelay);
            var startTime = Time.time;
            var currentTime = Time.time;
            DestoryGameObjects(_secondCasting);
            while ((currentTime - startTime) * 1000 <= _endDelayTime)
            {
                yield return null;
                currentTime = Time.time;
            }
            SetNextState();
            yield break;
        }


        protected override IEnumerator OnFrontDelay()
        {
            ApplyMovementSpeed(State.FrontDelay);
            var startTime = Time.time;
            var currentTime = Time.time;
            bool isCanceled = false;

            while ((currentTime - startTime) * 1000 <= _frontDelayTime)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    isCanceled = true;
                    break;
                }
                yield return null;
                currentTime = Time.time;
            }

            if (isCanceled == true)
            {
                SetState(State.Release);
            }

            SetNextState();
            yield break;
        }

        protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
        {

        }

        protected override IEnumerator OnReadyToUse()
        {
            var isCanceled = false;

            var MousePos = GetMousePosition();

            ActiveGameObjects(_secondCasting);


            yield return new WaitUntil(() =>
            {
                MousePos = GetMousePosition();

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    isCanceled = true;
                    return isCanceled;
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    _endMousePoint = MousePos;
                    return true;
                }

                TranslateGameObjects(_secondCasting, MousePos);
                return isCanceled;
            });

            if (isCanceled)
            {
                SetState(State.Canceled);
                yield break;
            }

            SetParentGameObjects(_secondCasting, "UiObject");


            SetNextState();
        }

        protected override IEnumerator OnRelease()
        {
            ApplyMovementSpeed(State.Release);
            Destroy(gameObject);
            yield return null;
        }

        protected override IEnumerator OnUse()
        {
            ApplyMovementSpeed(State.Use);
            _player.CharacterAnimatorSync.Animator.SetTrigger("RandomDrop");
            // 移대찓???꾨줈 ?앹꽦 ?섎룄濡??섍린 ?꾪빐??y媛믪쓣 10???뷀빐以ъ뒿?덈떎.
            var mainCameraPos = Camera.main.transform.position + new Vector3(0.0f, 10.0f, 0.0f);
            var createObjectPos = _endMousePoint + new Vector3(0.0f, mainCameraPos.y, 0.0f);
            var obj = InGameSkillManager.Instance.CreateSkillObject(_targetId, createObjectPos);
            var UI = InGameSkillManager.Instance.CreateSkillUI("DropUI", createObjectPos);


            UI.GetComponent<DropUI>().InitDropUI(obj);
            StartCoroutine(GenerateBuff(obj));

            UI.transform.SetParent(GameObject.Find("UiObject").transform);
            UI.transform.SetPositionAndRotation(_secondCasting[0].transform.position, _secondCasting[0].transform.rotation);
            UI.transform.localScale = _secondCasting[0].transform.localScale;

            Debug.Log(UI.transform.parent.name);



            SetNextState();
            yield break;
        }


        private IEnumerator GenerateBuff(GameObject Obj)
        {
            yield return new WaitForSeconds(0.03f);
            Obj.GetComponent<Actor>().BuffController.GenerateBuff(_buffOnCollision[0]);
        }


        private void ActiveGameObjects(IEnumerable<GameObject> gameObjects, bool isActive = true)
        {
            foreach (var gameObj in gameObjects)
            {
                gameObj.SetActive(isActive);
            }
        }



        private void TranslateGameObjects(IEnumerable<GameObject> gameObjects, Vector3 position)
        {
            foreach (var gameObj in gameObjects)
            {
                gameObj.transform.position = position;
            }
        }


        private void ScaleGameObjects(IEnumerable<GameObject> gameObjects, Vector3 scale)
        {
            foreach (var gameObj in gameObjects)
            {
                gameObj.transform.localScale = scale;
            }
        }

        private void SetParentGameObjects(IEnumerable<GameObject> gameObjects, string transformName)
        {
            foreach (var gameObj in gameObjects)
            {
                gameObj.transform.SetParent(GameObject.Find(transformName).transform);
            }
        }

        private void DestoryGameObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObj in gameObjects)
            {
                Destroy(gameObj);
            }
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


        private bool SetStartPos()
        {
            var startPos = GetMousePosition();
            ///?꾩쭅 ?섏?怨??뚯떛?섎뒗 遺遺꾩쓣 ?댄빐瑜?紐삵빐??
            ///吏곸젒 怨꾩궛?댁꽌 ?ш린瑜?留욎톬?듬땲??
            if (Vector3.Distance(startPos, transform.position) > _range * 10.0f)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     ?쒕엻?섎뒗 ?ㅻ툕?앺듃媛 ?뺣쪧???곕씪 ?⑥뼱吏?붾뜲 ???곗씠?곕? 遺덈윭?ㅻ뒗 ?⑥닔?낅땲??
        ///     理쒖큹 ?ㅽ뻾 ???쒕쾲留??몄텧?섎㈃ ?섍린 ?뚮Ц??愿?⑦븯???덉쇅泥섎━瑜??섏??듬땲??
        /// </summary>
        private void SetDropObjectCSVData()
        {
            if (s_dropObjectPercentageData.Count > 0)
            {
                return;
            }

            // CSV ?곗씠???곸슜
            var csvData = CSVUtil.GetData("DropSkillDropObjectDataTable");
            csvData.Get<float>("Obj_ID", out var idList);
            csvData.Get<float>("Percentage", out var percentageList);

            for (var i = 0; i < idList.Count; ++i)
            {
                s_dropObjectPercentageData.Add((int)idList[i], percentageList[i] / 100.0f);
            }
        }

        private int GetRandomDropObjectID()
        {
            var randomValue = Random.Range(0.0f, 1.0f);
            var totalPercentage = 0.0f;
            foreach (var percentageDataPair in s_dropObjectPercentageData)
            {
                totalPercentage += percentageDataPair.Value;
                // ?뺣쪧 踰붿쐞 ?덉뿉 ?덉쑝硫??뱀꺼
                if (randomValue <= totalPercentage)
                {
                    Debug.Log(percentageDataPair.Key);
                    return percentageDataPair.Key;
                }
            }

            return 0;
        }

    }
}
