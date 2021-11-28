using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EsperFightersCup
{
    public class DropSkillObject : SkillObject
    {
        private class DropObjectData
        {
            public int TargetID { get; set; }
            public float DropRate { get; set; }
        }

        [SerializeField]
        private GameObject[] _secondCasting;

        private List<DropObjectData> _dropObjects;
        private float _secondRange;
        private Vector3 _mouseEndPoint;

        protected override void OnIntializeSkill()
        {
            base.OnIntializeSkill();

            LoadDropObjectCSVData();
            _secondRange = Range - 0.15f;
        }

        protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
        {
            bool isCanceled = false;
            Vector3 mousePos;

            GameObjectUtil.ScaleGameObjects(_secondCasting, new Vector3(_secondRange * 2.0f, 1.0f, _secondRange * 2.0f));
            GameObjectUtil.ActiveGameObjects(_secondCasting);

            await UniTask.WaitUntil(() =>
            {
                mousePos = GetMousePosition();

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    isCanceled = true;
                    return isCanceled;
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    _mouseEndPoint = mousePos;
                    return true;
                }

                GameObjectUtil.TranslateGameObjects(_secondCasting, mousePos);
                return isCanceled;

            }, cancellationToken: cancellation);

            if (isCanceled)
            {
                return false;
            }

            GameObjectUtil.SetParentGameObjects(_secondCasting, "UiObject");
            return true;
        }

        protected override void BeforeFrontDelay()
        {
        }

        protected override async UniTask OnUseAsync()
        {
            AuthorPlayer.Animator.SetTrigger("RandomDrop");
            // 카메라 위로 생성 하도록 하기 위해서 y값을 10을 더해줬습니다.
            var mainCameraPos = Camera.main.transform.position + new Vector3(0.0f, 10.0f, 0.0f);
            var createObjectPos = _mouseEndPoint + new Vector3(0.0f, mainCameraPos.y, 0.0f);

            var id = GetRandomDropObjectID();
            var obj = InGameSkillManager.Instance.CreateSkillObject(id, createObjectPos);
            var ui = InGameSkillManager.Instance.CreateSkillUI("DropUI", createObjectPos);

            ui.GetComponent<DropUI>().InitDropUI(obj);
            StartCoroutine(GenerateBuff(obj));

            ui.transform.SetParent(GameObject.Find("UiObject").transform);
            ui.transform.SetPositionAndRotation(_secondCasting[0].transform.position, _secondCasting[0].transform.rotation);
            ui.transform.localScale = _secondCasting[0].transform.localScale;

            await UniTask.Yield();
        }

        protected override void BeforeEndDelay()
        {
            GameObjectUtil.DestoryGameObjects(_secondCasting);
        }

        protected override void OnRelease()
        {
        }

        protected override void OnCancel()
        {
        }

        private IEnumerator GenerateBuff(GameObject obj)
        {
            yield return new WaitForSeconds(0.03f);
            obj.GetComponent<Actor>().BuffController.GenerateBuff(_buffOnCollision[0]);
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
            return Vector3.Distance(startPos, transform.position) > Range;
        }

        /// <summary>
        /// 드랍되는 오브젝트가 확률에 따라 떨어지는데 이 데이터를 불러오는 함수입니다.
        /// 최초 실행 시 한번만 호출하면 되기 때문에 관련하여 예외처리를 하였습니다.
        /// </summary>
        private void LoadDropObjectCSVData()
        {
            if (_dropObjects != null)
            {
                return;
            }

            var csvData = CSVUtil.GetData("DropSkillDropObjectDataTable");
            if (!csvData.Get<int>("Obj_ID", out var targetIds))
            {
                return;
            }

            _dropObjects = new List<DropObjectData>(targetIds.Count);
            for (int i = 0; i < targetIds.Count; i++)
            {
                _dropObjects[i].TargetID = targetIds[i];
            }

            if (csvData.Get<float>("Obj_DropRate", out var dropRates))
            {
                for (int i = 0; i < dropRates.Count; i++)
                {
                    _dropObjects[i].DropRate = dropRates[i];
                }
            }

            // 백분율로 평준화 후 오름차순으로 정렬
            float totalWeight = 0f;
            foreach (var obj in _dropObjects)
            {
                totalWeight += obj.DropRate;
            }
            foreach (var obj in _dropObjects)
            {
                obj.DropRate /= totalWeight;
            }

            _dropObjects = _dropObjects.OrderBy(x => x.DropRate).ToList();
        }

        private int GetRandomDropObjectID()
        {
            // TODO: 드랍오브젝트 선택을 Drop버프로 옮겨야 함
            // 출처: https://m.blog.naver.com/occidere/222024179048
            float pivot = Random.value;

            float acc = 0f;
            foreach (var obj in _dropObjects)
            {
                acc += obj.DropRate;
                if (pivot <= acc)
                {
                    return obj.TargetID;
                }
            }
            return _dropObjects[0].TargetID;
        }
    }
}
