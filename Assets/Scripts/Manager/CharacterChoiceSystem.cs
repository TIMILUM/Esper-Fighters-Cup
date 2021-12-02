using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup.Manager
{
    public class CharacterChoiceSystem : PunEventSingleton<CharacterChoiceSystem>
    {
        public ACharacter.Type ChooseCharacter { get; set; }

        private int _count;

        public bool Submit()
        {
            if (ChooseCharacter == ACharacter.Type.None)
            {
                return false;
            }

            return PhotonNetwork.LocalPlayer.SetCustomProperty(CustomPropertyKeys.PlayerCharacterType, (int)ChooseCharacter);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!changedProps.TryGetValue(CustomPropertyKeys.PlayerCharacterType, out var value) || value is null)
            {
                return;
            }

            _count++;
            if (PhotonNetwork.IsMasterClient && _count >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                SetPaletteOfRoomPlayers();

                DOTween.Sequence()
                    .SetLink(gameObject)
                    .AppendInterval(3.0f)
                    .AppendCallback(() => PhotonNetwork.LoadLevel("GameScene"));
            }
        }

        private void SetPaletteOfRoomPlayers()
        {
            var paletteIndex = new Dictionary<int, int>();

            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                var characterType = (int)player.CustomProperties[CustomPropertyKeys.PlayerCharacterType];
                if (paletteIndex.TryGetValue(characterType, out int index))
                {
                    index++;
                }
                else
                {
                    index = 0;
                }

                paletteIndex[characterType] = index;
                player.SetCustomProperty(CustomPropertyKeys.PlayerPalette, index);
            }
        }
    }
}
