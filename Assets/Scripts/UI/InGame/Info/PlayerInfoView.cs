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

            // MasterClient가 무조건 왼쪽으로 가게 함
            if (targetPlayer.IsMasterClient && _targetPlayerIndex == 1)
            {
                return;
            }

            var winPoint = (int)value;
            for (int i = 0; i < _winPoints.Length; i++)
            {
                _winPoints[i].gameObject.SetActive(i < winPoint);
            }
        }

        private void Update()
        {
            if (_targetPlayer == null)
            {
                return;
            }
        }
    }
}
