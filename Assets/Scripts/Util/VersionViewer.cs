using UnityEngine;

namespace EsperFightersCup.Util
{
    public sealed class VersionViewer : Singleton<VersionViewer>
    {
        private GUIStyle _whiteStyle;
        private string _version = string.Empty;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitInstance()
        {
            if (Debug.isDebugBuild)
            {
                CreateNewSingleton();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _version = $"Game Version : v{Application.version}";
            _whiteStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                normal = new GUIStyleState { textColor = Color.white },
                clipping = TextClipping.Overflow,
            };

            DontDestroyOnLoad(gameObject);
        }

        private void OnGUI()
        {
            // GUI.Label(new Rect(Screen.width / 2, Screen.height - 10, 0, 0), _version, _blackStyle);
            GUI.Label(new Rect(Screen.width / 2, Screen.height - 10, 0, 0), _version, _whiteStyle);
        }
    }
}
