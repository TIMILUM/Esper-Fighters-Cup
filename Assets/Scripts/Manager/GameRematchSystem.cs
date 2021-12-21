using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup.Manager
{
    public class GameRematchSystem : PunEventSingleton<GameRematchSystem>
    {
        private bool _otherPlayerRematch;
        private bool _localPlayerRematch;

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!changedProps.TryGetValue(CustomPropertyKeys.PlayerGameRematch, out var value))
            {
                return;
            }

            var rematch = (bool)value;

            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                _localPlayerRematch = rematch;
            }
            else
            {
                _otherPlayerRematch = rematch;
            }

            if (PhotonNetwork.IsMasterClient && _localPlayerRematch && _otherPlayerRematch && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                PhotonNetwork.LoadLevel("CharacterChoiceScene");
            }
        }
    }
}
