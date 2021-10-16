using UnityEngine;

namespace EsperFightersCup.UI.Settings
{
    public class ScreenModeOption : MonoBehaviour
    {
        [SerializeField] private FullScreenMode _screenMode;

        public FullScreenMode ScreenMode => _screenMode;
    }
}
