using UnityEngine;

namespace EsperFightersCup
{
    public sealed class VersionViewer : MonoBehaviour
    {
        private static VersionViewer s_instance = null;

        private GUIStyle _whiteStyle;
        private GUIStyle _blackStyle;
        private string _version = string.Empty;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitVersionViewer()
        {
            if (s_instance is null && Debug.isDebugBuild)
            {
                var viewer = new GameObject("Version Viewer").AddComponent<VersionViewer>();
                DontDestroyOnLoad(viewer.gameObject);
            }
        }

        private void Awake()
        {
            if (s_instance is null)
            {
                s_instance = this;
                _version = $"Game Version : v{Application.version}";
                _whiteStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14,
                    normal = new GUIStyleState { textColor = Color.white },
                    clipping = TextClipping.Overflow,
                };
                _blackStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState { textColor = Color.black },
                    clipping = TextClipping.Overflow,
                };


                return;
            }

            Destroy(this);
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height - 10, 0, 0), _version, _blackStyle);
            GUI.Label(new Rect(Screen.width / 2, Screen.height - 10, 0, 0), _version, _whiteStyle);
        }
    }
}
