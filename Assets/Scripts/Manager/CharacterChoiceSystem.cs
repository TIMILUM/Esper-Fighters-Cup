using DG.Tweening;
using EsperFightersCup.Util;
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
            var prop = new Hashtable { [CustomPropertyKeys.PlayerCharacterType] = (int)ChooseCharacter };
            return PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!changedProps.TryGetValue(CustomPropertyKeys.PlayerCharacterType, out var _))
            {
                return;
            }

            _count++;
            if (PhotonNetwork.IsMasterClient && _count >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                DOTween.Sequence()
                    .SetLink(gameObject)
                    .AppendInterval(3.0f)
                    .AppendCallback(() => PhotonNetwork.LoadLevel("GameScene"));
            }
        }
    }
}
