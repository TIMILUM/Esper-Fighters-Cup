using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace EsperFightersCup.Net
{
    public class PacketObserver : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        private static PacketObserver s_instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void CreateInstance()
        {
            s_instance ??= new GameObject("Packet Observer").AddComponent<PacketObserver>();
        }

        private void Awake()
        {
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
            if (photonEvent.Code >= 200)
            {
                return;
            }

            Debug.Log($"<color=grey>[Packet Check] received: {photonEvent.Code} from actor {photonEvent.Sender}</color>");
        }
    }
}
