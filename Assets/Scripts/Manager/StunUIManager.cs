using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EsperFightersCup
{
    public class StunUIManager : Singleton<StunUIManager>
    {
        [SerializeField]
        private Canvas _screenCanvas;
        [SerializeField]
        private StunUI _stunUI;
        [SerializeField]
        private Vector2 _offset;

        public void PlayStunUI(float duration, Transform target)
        {
            PlayStunUIAsync(duration, target).Forget();
        }

        private async UniTask PlayStunUIAsync(float duration, Transform target)
        {
            if (!target.gameObject)
            {
                return;
            }

            var ui = Instantiate(_stunUI, _screenCanvas.transform);

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                if (!target)
                {
                    break;
                }

                var pos = target.position;
                var offset = new Vector3(_offset.x, _offset.y, 0);
                ui.transform.position = Camera.main.WorldToScreenPoint(pos) + offset;
                ui.SetAmount(elapsedTime / duration);

                await UniTask.NextFrame();
                elapsedTime += Time.deltaTime;
            }

            Destroy(ui.gameObject);
        }
    }
}
