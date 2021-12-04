using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup
{
    public class RoomManager : PunEventSingleton<RoomManager>
    {
        public event UnityAction<string> OnRoomCreated;
        public event UnityAction<string> OnRoomCreateFaild;
        public event UnityAction OnRoomJoined;
        public event UnityAction<Player> OnPlayerJoined;
        public event UnityAction<Player> OnPlayerLeft;
        public event UnityAction OnGameStart;

        public string RoomCode { get; private set; }

        private void Start()
        {
            if (!PhotonNetwork.InRoom)
            {
                UniTask.NextFrame().ContinueWith(CreateNewRoom).Forget();
                return;
            }
        }

        public void StartGame()
        {
            var room = PhotonNetwork.CurrentRoom;
            if (PhotonNetwork.IsMasterClient && room.PlayerCount == room.MaxPlayers)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { [CustomPropertyKeys.GameStarted] = true });
            }
        }

        public void ExitRoom()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                SceneManager.LoadScene("LobbyScene");
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(CustomPropertyKeys.GameStarted, out var value))
            {
                if ((bool)value == false)
                {
                    return;
                }

                OnGameStart?.Invoke();
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    CoroutineTimer.SetTimerOnce(() => PhotonNetwork.LoadLevel("CharacterChoiceScene"), 2f);
                }
            }
        }

        private void CreateNewRoom()
        {
            var code = Random.Range(0, 10000).ToString().PadLeft(4, '0');

            var result = PhotonNetwork.CreateRoom(code,
                roomOptions: PhotonOptions.DefaultRoomOption, typedLobby: PhotonOptions.CustomMatchLobby);

            if (!result)
            {
                OnRoomCreateFaild?.Invoke(string.Empty);
                return;
            }

            RoomCode = code;
            Debug.Log($"RoomCode: {code}");
        }

        public override void OnCreatedRoom()
        {
            Debug.Log($"Room Create with room code: {RoomCode}");
            OnRoomCreated?.Invoke(RoomCode);
        }

        public override void OnJoinedRoom()
        {
            OnRoomJoined?.Invoke();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            if (returnCode == 32766)
            {
                Debug.LogWarning($"RoomCode [{RoomCode}] is already exist. Try again.");
                UniTask.Delay(100).ContinueWith(CreateNewRoom).Forget();
                return;
            }
            OnRoomCreateFaild?.Invoke(message);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            OnPlayerJoined?.Invoke(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            OnPlayerLeft?.Invoke(otherPlayer);
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
