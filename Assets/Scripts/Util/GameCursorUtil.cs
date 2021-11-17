using UnityEngine;

namespace EsperFightersCup
{
    public class GameCursorUtil : MonoBehaviour
    {
        [SerializeField]
        private Texture2D _cursorTexture;
        [SerializeField]
        private Vector2 _cursorStartPosition;
        private static bool _isSetCursor = false;

        private void Start()
        {
            if (_isSetCursor)
            {
                return;
            }

            SetCursorTexture();
            SetVisible(true);
        }

        private void SetCursorTexture()
        {
            _cursorStartPosition.x += _cursorTexture.width / 2.0f;
            _cursorStartPosition.y += _cursorTexture.height / 2.0f;

            Cursor.SetCursor(_cursorTexture, _cursorStartPosition, CursorMode.Auto);
            _isSetCursor = true;
        }

        public static void SetVisible(bool isVisible)
        {
            Cursor.visible = isVisible;
        }
    }
}
