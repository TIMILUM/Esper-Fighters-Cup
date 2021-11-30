using System.Linq;
using EsperFightersCup.Net;
using EsperFightersCup.UI.InGame.Skill;
using ExitGames.Client.Photon;
using UnityEngine;

namespace EsperFightersCup
{
    public class GameUIManager : PunEventSingleton<GameUIManager>
    {
        [System.Serializable]
        private class GameUI
        {
            [SerializeField] private string _name;
            [SerializeField] private SkillUI _prefab;

            public string Name => _name;
            public SkillUI Prefab => _prefab;
        }

        [Tooltip("UI가 생성될 Y 위치입니다.")]
        [SerializeField] private Transform _uiParent;
        [SerializeField] private float _positionY = 0.01f;
        [SerializeField] private GameUI[] _uiList;


        protected override void Awake()
        {
            base.Awake();
            _uiParent.SetPositionAndRotation(new Vector3(0, _positionY, 0), Quaternion.identity);
        }

        public void Play(string name, Vector3 position, float rotationY, Vector2 scale, float duration = 0f, int viewID = -1)
        {
            Play(name, new Vector2(position.x, position.z), rotationY, scale, duration, viewID);
        }

        /// <summary>
        /// 모든 클라이언트에 UI를 생성합니다. UI 오브젝트에 있는 스크립트를 통해서만 조작 가능합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="rotationY"></param>
        /// <param name="scale"></param>
        public void Play(string name, Vector2 position, float rotationY, Vector2 scale, float duration = 0f, int viewID = -1)
        {
            EventSender.Broadcast(new GameUIPlayArguments(name, position, rotationY, scale, duration, viewID), SendOptions.SendReliable);
        }

        public SkillUI PlayLocal(string name, Vector3 position, float rotationY, Vector2 scale, float duration = 0f, int viewID = -1)
        {
            return PlayLocal(name, new Vector2(position.x, position.z), rotationY, scale, duration, viewID);
        }

        /// <summary>
        /// 로컬에만 UI를 생성합니다. 반환값으로 받는 UI 오브젝트로 자유롭게 조작 가능합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="rotationY"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public SkillUI PlayLocal(string name, Vector2 position, float rotationY, Vector2 scale, float duration = 0f, int viewID = -1)
        {
            return CloneUI(new GameUIPlayArguments(name, position, rotationY, scale, duration, viewID));
        }

        private SkillUI CloneUI(GameUIPlayArguments args)
        {
            var ui = _uiList.FirstOrDefault(x => x.Name == args.Name);
            if (ui is null)
            {
                Debug.LogError("이름과 일치하는 UI가 없습니다.");
                return null;
            }

            var clone = Instantiate(ui.Prefab, _uiParent);
            clone.Init(args);

            return clone;
        }

        protected override void OnGameEventReceived(GameEventArguments args)
        {
            if (args.Code != EventCode.PlayGameUI)
            {
                return;
            }

            var data = (GameUIPlayArguments)args.EventData;
            CloneUI(data);
        }
    }
}
