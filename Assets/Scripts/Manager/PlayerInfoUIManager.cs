using EsperFightersCup.UI;
using UnityEngine;

namespace EsperFightersCup
{
    public class PlayerInfoUIManager : Singleton<PlayerInfoUIManager>
    {
        [SerializeField] private GameObject _topBanner;
        [SerializeField] private PlayerInfoView _leftView;
        [SerializeField] private PlayerInfoView _rightView;

        public void SetVisible(bool visible)
        {
            _topBanner.SetActive(visible);
        }

        public void SetPlayer(APlayer player)
        {
            if (player == null)
            {
                return;
            }

            if (player.photonView.Owner.IsMasterClient)
            {
                _leftView.TargetPlayer = player;
            }
            else
            {
                _rightView.TargetPlayer = player;
            }
        }
    }
}
