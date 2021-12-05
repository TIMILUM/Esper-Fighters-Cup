using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    public class PlayerInfoView : MonoBehaviourPunCallbacks
    {
        [SerializeField, Min(0f)]
        private int _targetPlayerIndex;
        [SerializeField]
        private Text _nameText;
        [SerializeField]
        private PaletteSwapUI _healthBarBackground;
        [SerializeField]
        private PaletteSwapUI _healthBarForground;
        [SerializeField]
        private PaletteSwapUI[] _winPoints;

        private APlayer _targetPlayer;
        private List<PaletteSwapUI> _swapUIs;

        public APlayer TargetPlayer
        {
            get => _targetPlayer;
            set
            {
                _targetPlayer = value;

                if (value == null)
                {
                    return;
                }
                _nameText.text = value.photonView.Owner.NickName;
                SetWinPoint(value.WinPoint);
                _swapUIs.ForEach(ui => ui.Swap(value.CharacterType, value.PaletteIndex));
            }
        }

        private void Awake()
        {
            _swapUIs = new List<PaletteSwapUI>
            {
                _healthBarBackground,
                _healthBarForground
            };
            _swapUIs.AddRange(_winPoints);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!changedProps.TryGetValue(CustomPropertyKeys.PlayerWinPoint, out var value))
            {
                return;
            }

            if (TargetPlayer == null || TargetPlayer.photonView.OwnerActorNr != targetPlayer.ActorNumber)
            {
                return;
            }

            var winPoint = (int)value;
            SetWinPoint(winPoint);
        }

        private void Update()
        {
            if (TargetPlayer == null)
            {
                return;
            }

            var hp = TargetPlayer.HP;
            var maxHp = TargetPlayer.MaxHP;
            _healthBarForground.TargetImage.fillAmount = hp / (float)maxHp;
            /*
            var amount = (float)System.Math.Round(hp / (double)maxHp, 3);
            if (amount != _healthBarForground.TargetImage.fillAmount)
            {
                _healthBarBackground.TargetImage.DOFillAmount(amount, 0.25f);
            }
            */
        }

        private void SetWinPoint(int winPoint)
        {
            for (int i = 0; i < _winPoints.Length; i++)
            {
                _winPoints[i].gameObject.SetActive(i < winPoint);
            }
        }
    }
}
