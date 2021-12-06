using EsperFightersCup.UI;
using UnityEngine;

namespace EsperFightersCup
{
    public class PopupManager : Singleton<PopupManager>
    {
        [SerializeField]
        private BasicPopup _basicPopup;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public BasicPopup CreateNewBasicPopup()
        {
            var popup = Instantiate(_basicPopup, transform);
            return popup;
        }
    }
}
