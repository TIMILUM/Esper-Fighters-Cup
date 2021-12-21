using UnityEngine;

namespace EsperFightersCup
{
    public class GameCursorUtil : MonoBehaviour
    {
        /*
        [SerializeField]
        private Texture2D _cursorTexture;

        private void Start()
        {
            if (s_isSetCursor)
            {
                return;
            }

            SetCursorTexture();
            SetVisible(true);
        }

        private void SetCursorTexture()
        {
            Cursor.SetCursor(_cursorTexture, Vector2.zero, CursorMode.Auto);
            s_isSetCursor = true;
        }
        */

        public static void SetVisible(bool isVisible)
        {
            Cursor.visible = isVisible;
        }
    }
}
