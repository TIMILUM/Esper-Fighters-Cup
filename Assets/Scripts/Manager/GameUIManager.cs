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

        public void PlaySync(Actor author, string name, Vector3 position, Vector2 scale, float rotationY = 0f, float duration = 0f)
        {
            var uiPosition = new Vector2(position.x, position.z);
            PlaySync(author, name, uiPosition, scale, rotationY, duration);
        }

        /// <summary>
        /// 모든 클라이언트에 UI를 생성합니다. UI 오브젝트에 있는 스크립트를 통해서만 조작 가능합니다.
        /// </summary>
        public void PlaySync(Actor author, string name, Vector2 position, Vector2 scale, float rotationY = 0f, float duration = 0f)
        {
            var viewID = author == null ? -1 : author.photonView.ViewID;
            var args = new GameUIPlayArguments(name, position, rotationY, scale, duration, viewID);

            EventSender.Broadcast(in args, SendOptions.SendReliable);
        }

        public SkillUI PlayLocal(Actor author, string name, Vector3 position, Vector2 scale, float rotationY = 0f, float duration = 0f)
        {
            var uiPosition = new Vector2(position.x, position.z);
            return PlayLocal(author, name, uiPosition, scale, rotationY, duration);
        }

        /// <summary>
        /// 로컬에만 UI를 생성합니다. 반환값으로 받는 UI 오브젝트로 자유롭게 조작 가능합니다.
        /// </summary>
        public SkillUI PlayLocal(Actor author, string name, Vector2 position, Vector2 scale, float rotationY = 0f, float duration = 0f)
        {
            var viewID = author == null ? -1 : author.photonView.ViewID;
            var args = new GameUIPlayArguments(name, position, rotationY, scale, duration, viewID);
            return CloneUI(in args);
        }

        private SkillUI CloneUI(in GameUIPlayArguments args)
        {
            var name = args.Name;
            var ui = _uiList.FirstOrDefault(x => x.Name == name);
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
            CloneUI(in data);
        }
    }
}
