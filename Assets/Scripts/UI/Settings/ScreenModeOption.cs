using UnityEngine;

namespace EsperFightersCup.UI
{
    public class ScreenModeOption : MonoBehaviour
    {
        [SerializeField] private FullScreenMode _screenMode;

        public FullScreenMode ScreenMode => _screenMode;
    }
}
