using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using EsperFightersCup.UI.InGame.Skill;
using UnityEngine;

namespace EsperFightersCup
{
    public class RandDropSkillObject : SkillObject
    {
        private class DropObjectData
        {
            public int TargetID { get; set; }
            public float DropRate { get; set; }
        }

        private const string CreateSound = "event:/SFX/Elena_Skill/SFX_RandDrop";

        private DropObjectData[] _dropObjects;
        private float _dropDelay;

        private Vector2 _uiSize;
        private SkillUI _rangeUI;
        private SkillUI _castUI;

        protected override void OnInitializeSkill()
        {
            base.OnInitializeSkill();
            LoadDropObjectCSVData();

            _uiSize = Size * 0.1f;

            var rangeSize = new Vector2(Range, Range) * 2f;
            _rangeUI = GameUIManager.Instance.PlayLocal("Skill_Range", transform.position, 0f, rangeSize * 0.1f);
            _castUI = GameUIManager.Instance.PlayLocal("RandDrop_Casting", transform.position, 0f, _uiSize);

            GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, false);
            GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);
        }

        protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
        {
            var isCanceled = false;

            _rangeUI.ChangeTarget(AuthorPlayer.photonView.ViewID);
            GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, true);

            await UniTask.WaitUntil(() =>
            {
                var mousePos = GetMousePosition();
                var distance = Vector3.Distance(Author.transform.position, mousePos);

                if (distance < Range)
                {
                    if (!_castUI.gameObject.activeInHierarchy)
                    {
                        GameObjectUtil.ActiveGameObject(_castUI.gameObject, true);
                    }
                    GameObjectUtil.TranslateGameObject(_castUI.gameObject, mousePos);
                }
                else
                {
                    GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);
                }

                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    isCanceled = true;
                }
                else if (Input.GetKeyUp(InputKey))
                {
                    if (distance > Range)
                    {
                        isCanceled = true;
                    }
                    return true;
                }
                return isCanceled;

            }, cancellationToken: cancellation);

            GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, false);
            GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);

            return !isCanceled;
        }

        protected override void BeforeFrontDelay()
        {
            AuthorPlayer.Animator.SetTrigger("RandomDrop");
        }

        protected override async UniTask OnUseAsync()
        {
            InstantiateRandomDropObjectAsync(_castUI.transform.position).Forget();
            await UniTask.Yield();
        }

        protected override void BeforeEndDelay()
        {
        }

        protected override void OnRelease()
        {
            ReleaseObjects();
        }

        protected override void OnCancel()
        {
            ReleaseObjects();
        }

        private async UniTask InstantiateRandomDropObjectAsync(Vector3 position)
        {
            var audioInstance = FMODUnity.RuntimeManager.CreateInstance(CreateSound);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(audioInstance, gameObject.transform, Author.Rigidbody);
            audioInstance.start();

            var duration = _dropDelay * 0.001f;
            GameUIManager.Instance.Play("RandDrop_Range", position, 0f, _uiSize, duration);
            await UniTask.Delay((int)_dropDelay);

            // 카메라 위로 생성 하도록 하기 위해서 y값을 10을 더해줬습니다.
            var mainCameraPos = Camera.main.transform.position + new Vector3(0.0f, 10.0f, 0.0f);
            var createObjectPos = position + new Vector3(0.0f, mainCameraPos.y, 0.0f);

            var id = GetRandomDropObjectID();
            var obj = InGameSkillManager.Instance.CreateSkillObject(id, createObjectPos).GetComponent<AStaticObject>();
            if (obj == null)
            {
                Debug.LogError("DropObject가 AStaticObject가 아닙니다!");
                return;
            }

            var objPV = obj.photonView;
            if (objPV == null)
            {
                Debug.LogError("DropObject에 photonView를 찾지 못했습니다!");
                return;
            }

            await UniTask.DelayFrame(1);
            obj.BuffController.GenerateBuff(new BuffObject.BuffStruct
            {
                Type = BuffObject.Type.Falling,
                ValueFloat = new float[] { Damage, StunGroggyDuration }
            });

            audioInstance.setParameterByName("Step", 1f);
            audioInstance.release();
            audioInstance.clearHandle();
        }

        private void ReleaseObjects()
        {
            GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, false);
            GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);
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

            _dropDelay = GetCSVData<float>("Skill_Effect_Data_1");

            var csvData = CSVUtil.GetData("DropSkillDropObjectDataTable");
            if (!csvData.Get<float>("Obj_ID", out var targetIds))
            {
                return;
            }

            _dropObjects = new DropObjectData[targetIds.Count];
            for (int i = 0; i < targetIds.Count; i++)
            {
                _dropObjects[i] = new DropObjectData
                {
                    TargetID = (int)targetIds[i]
                };
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

            _dropObjects = _dropObjects.OrderBy(x => x.DropRate).ToArray();
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
