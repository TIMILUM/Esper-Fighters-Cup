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
    public sealed class GameEventListener : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        /// <summary>
        /// <see cref="GameEventListener"/>의 인스턴스를 가져옵니다.
        /// </summary>
        public static GameEventListener Instance
        {
            get
            {
                if (!s_instance)
                {
                    CreateInstance();
                }
                return s_instance;
            }
        }

        private static GameEventListener s_instance;

        [RuntimeInitializeOnLoadMethod]
        public static void CreateInstance()
        {
            if (!s_instance)
            {
                s_instance = new GameObject("Game Event Listener").AddComponent<GameEventListener>();
            }
        }

        /// <summary>
        /// 게임이벤트를 받으면 발생합니다.
        /// </summary>
        public event UnityAction<GameEventArguments> GameEventReceived;

        private void Awake()
        {
            // BUG: 유니티 에디터 상에서 게임 종료 시 OnDestroy가 호출될 때 또 Awake가 호출되는 바람에 에러가 남
            Debug.Log("Try create Event Listener", gameObject);
            if (s_instance)
            {
                Destroy(gameObject);
                return;
            }
            s_instance = this;
            DontDestroyOnLoad(gameObject);

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
