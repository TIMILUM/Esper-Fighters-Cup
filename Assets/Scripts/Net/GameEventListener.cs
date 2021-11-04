using EsperFightersCup.Util;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace EsperFightersCup.Net
{
    /// <summary>
    /// RaiseEvent로 받은 데이터를 제공합니다.
    /// </summary>
    public struct GameEventArguments
    {
        /// <summary>
        /// 게임이벤트 코드
        /// </summary>
        public byte Code { get; }

        /// <summary>
        /// 게임이벤트를 보낸 ActorNumber
        /// </summary>
        public int Sender { get; }

        /// <summary>
        /// 게임이벤트 데이터
        /// </summary>
        public IGameEvent EventData { get; }

        public GameEventArguments(byte code, int sender, IGameEvent eventData)
        {
            Code = code;
            Sender = sender;
            EventData = eventData;
        }
    }

    /// <summary>
    /// 이벤트를 받아서 가공 후 뿌리는 매니저 클래스입니다.
    /// </summary>
    public sealed class GameEventListener : Singleton<GameEventListener>, IOnEventCallback
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitInstance()
        {
            CreateNewSingletonObject();
        }

        /// <summary>
        /// 게임이벤트를 받으면 발생합니다.
        /// </summary>
        public event UnityAction<GameEventArguments> GameEventReceived;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code >= 200) // Photon의 사용자 지정 이벤트는 0~199
            {
                return;
            }

            Debug.Log($"<color=grey>[Packet Check] received: {photonEvent.Code} from actor {photonEvent.Sender}</color>");
            var receivedEvent = PacketSerializer.Deserialize((byte[])photonEvent.CustomData);
            GameEventReceived?.Invoke(new GameEventArguments(photonEvent.Code, photonEvent.Sender, receivedEvent));
        }
    }
}
