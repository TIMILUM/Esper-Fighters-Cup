using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using Photon.Pun;

namespace EsperFightersCup
{
    public class RoomFindManager : PunEventSingleton<RoomFindManager>
    {
        private readonly ConcurrentQueue<UniTaskCompletionSource<bool>> _tasks = new ConcurrentQueue<UniTaskCompletionSource<bool>>();

        private void Start()
        {
            PhotonNetwork.JoinLobby(PhotonOptions.CustomMatchLobby);
        }

        public UniTask<bool> FindRoomAsync(string roomCode)
        {
            if (!PhotonNetwork.JoinRoom(roomCode))
            {
                return new UniTask<bool>(false);
            }

            var source = new UniTaskCompletionSource<bool>();
            _tasks.Enqueue(source);
            return source.Task;
        }

        public override void OnJoinedRoom()
        {
            if (_tasks.TryDequeue(out var source))
            {
                source.TrySetResult(true);
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (_tasks.TryDequeue(out var source))
            {
                source.TrySetResult(false);
            }
        }
    }
}
