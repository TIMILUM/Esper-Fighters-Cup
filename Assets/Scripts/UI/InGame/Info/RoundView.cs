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

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(CustomPropertyKeys.GameRound, out var value))
            {
                var round = (int)value;

                if (_roundImages.Length == 0)
                {
                    return;
                }

                _roundImage.sprite = _roundImages[(round > _roundImages.Length ? _roundImages.Length : round) - 1];
            }
        }
    }
}
