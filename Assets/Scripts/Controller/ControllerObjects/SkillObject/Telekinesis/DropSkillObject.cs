using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public class DropSkillObject : SkillObject
    {

        private int _dropRate = 0;
        [SerializeField]
        private float _range;
        private float _secondrange;

        [SerializeField]
        private float _frontDelayTime;

        [SerializeField]
        private float _endDelayTime;

        [SerializeField]
        private GameObject _dropUIPrefab;



        [SerializeField]
        private GameObject[] _firstCasting;
        [SerializeField]
        private GameObject[] _secondCasting;


        private float _perSceond;
        private Vector3 _endMousePoint;

        protected override void Start()
        {

            base.Start();
            //_range = GetCSVData<float>("Range") * 0.001f;
            _range = 0.35f;
            _secondrange = 0.2f;
            _frontDelayTime = FrontDelayMilliseconds;
            _endDelayTime = EndDelayMilliseconds;

            ///아직 엘셀과 파싱하는 부분을 이해를 못해서 
            ///직접 계산해서 크기를 맞췄습니다.            
            ScaleGameObjects(_firstCasting, new Vector3(_range * 2.0f, 1.0f, _range * 2.0f));
            ScaleGameObjects(_secondCasting, new Vector3(_secondrange * 2.0f, 1.0f, _secondrange * 2.0f));

        }

        public override void OnPlayerHitEnter(GameObject other)
        {

        }

        protected override IEnumerator OnCanceled()
        {
            SetState(State.Release);
            yield return null;
        }

        protected override IEnumerator OnEndDelay()
        {

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
            ActiveGameObjects(_firstCasting);
            ActiveGameObjects(_secondCasting);


            yield return new WaitUntil(() =>
            {
                MousePos = GetMousePosition();
                TranslateGameObjects(_firstCasting, transform.position);
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    isCanceled = true;
                    return isCanceled;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (SetStartPos())
                    {
                        isCanceled = true;
                        return isCanceled;
                    }
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
            ActiveGameObjects(_firstCasting, false);

            SetNextState();
        }

        protected override IEnumerator OnRelease()
        {
            ActiveGameObjects(_firstCasting, false);
            Destroy(gameObject);
            yield return null;
        }

        protected override IEnumerator OnUse()
        {
            // 카메라 위로 생성 하도록 하기 위해서 y값을 10을 더해줬습니다.
            var mainCameraPos = Camera.main.transform.position + new Vector3(0.0f, 10.0f, 0.0f);
            var createObjectPos = _endMousePoint + new Vector3(0.0f, mainCameraPos.y, 0.0f);
            var obj = InGameSkillManager.Instance.CreateSkillObject("DropObject", createObjectPos);
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
            ///아직 엘셀과 파싱하는 부분을 이해를 못해서 
            ///직접 계산해서 크기를 맞췄습니다.
            if (Vector3.Distance(startPos, transform.position) > _range * 10.0f)
            {
                return true;
            }

            return false;
        }

    }
}
