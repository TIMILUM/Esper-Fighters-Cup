using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup.Manager
{
    /// <summary>
    /// <see cref="int"/> -> sender actor number, <see cref="ACharacter.Type"/> -> changed character type
    /// </summary>
    [System.Serializable]
    public class CharacterChangeEvent : UnityEvent<int, ACharacter.Type>
    {
    }

    [RequireComponent(typeof(PhotonView))]
    public class CharacterChoiceSystem : PunEventSingleton<CharacterChoiceSystem>
    {
        [SerializeField]
        private CharacterChangeEvent _onCharacterChanged;
        [SerializeField]
        private UnityEvent _onSubmitted;

        public (ACharacter.Type Type, int PaletteIndex) ChooseCharacter { get; set; }

        public event UnityAction<int, ACharacter.Type> OnCharacterChanged
        {
            add => _onCharacterChanged.AddListener(value);
            remove => _onCharacterChanged.RemoveListener(value);
        }

        public event UnityAction OnSubmitted
        {
            add => _onSubmitted.AddListener(value);
            remove => _onSubmitted.RemoveListener(value);
        }

        private int _count;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                ChangeCharacter((int)ACharacter.Type.Telekinesis);
            }
            else
            {
                ChangeCharacter((int)ACharacter.Type.Plank);
            }
        }

        public void ChangeCharacter(int type)
        {
            photonView.RPC(nameof(ChangeCharacterRPC), RpcTarget.All, type);
        }

        [PunRPC]
        private void ChangeCharacterRPC(int typeRaw, PhotonMessageInfo info)
        {
            _onCharacterChanged?.Invoke(info.Sender.ActorNumber, (ACharacter.Type)typeRaw);
        }

        public void Submit()
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
            {
                [CustomPropertyKeys.PlayerCharacterType] = (int)ChooseCharacter.Type,
                [CustomPropertyKeys.PlayerPalette] = ChooseCharacter.PaletteIndex
            });
            _onSubmitted?.Invoke();
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
                DOTween.Sequence()
                    .SetLink(gameObject)
                    .AppendInterval(3.0f)
                    .AppendCallback(() => PhotonNetwork.LoadLevel("GameScene"));
            }
        }

        /*
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
        */
    }
}
