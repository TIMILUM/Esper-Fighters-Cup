using System.Linq;
using EsperFightersCup.Util;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup.Manager
{
    public class GameRematchManager : PunEventSingleton<GameRematchManager>
    {
        public const string RematchPropKey = "rematch";

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps[RematchPropKey] is null)
            {
                return;
            }
            if (PhotonNetwork.IsMasterClient && CheckRematch())
            {
                foreach (var player in PhotonNetwork.CurrentRoom.Players)
                {
                    var activePlayer = player.Value;
                    activePlayer.SetCustomProperties(new Hashtable { [RematchPropKey] = false });
                }
                PhotonNetwork.LoadLevel("GameScene");
            }
        }

        private bool CheckRematch()
        {
            if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                return false;
            }

            // 플레이어 중에 리매치를 원하지 않는 플레이어가 있는지 검색
            var result = PhotonNetwork.CurrentRoom.Players.Any(
                player => !player.Value.CustomProperties.TryGetValue(RematchPropKey, out var check) || !(bool)check);

            return !result;
        }
    }
}
