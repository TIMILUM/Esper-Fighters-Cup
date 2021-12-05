using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    public class RoundView : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Image _roundImage;
        [SerializeField]
        private Sprite[] _roundImages;

        private void Start()
        {
            if (_roundImages.Length == 0)
            {
                return;
            }
            _roundImage.sprite = _roundImages[0];
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomPropertyKeys.GameRound, out var value))
            {
                var round = (int)value;
                SetRoundImage(round);
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(CustomPropertyKeys.GameRound, out var value))
            {
                var round = (int)value;
                SetRoundImage(round);
            }
        }

        private void SetRoundImage(int round)
        {
            if (_roundImages.Length == 0 || round == 0)
            {
                return;
            }

            _roundImage.sprite = _roundImages[(round > _roundImages.Length ? _roundImages.Length : round) - 1];
        }
    }
}
