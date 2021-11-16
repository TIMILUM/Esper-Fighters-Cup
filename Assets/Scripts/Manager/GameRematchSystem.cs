using EsperFightersCup.Util;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup.Manager
{
    public class GameRematchSystem : PunEventSingleton<GameRematchSystem>
    {
        private bool _otherPlayerRematch;

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!changedProps.TryGetValue(CustomPropertyKeys.PlayerGameRematch, out var value))
            {
                return;
            }

            var rematch = (bool)value;

            if (targetPlayer != PhotonNetwork.LocalPlayer)
            {
                _otherPlayerRematch = rematch;
                return;
            }

            if (rematch && _otherPlayerRematch)
            {
                PhotonNetwork.LoadLevel("CharacterChoiceScene");
            }
        }
    }
}
