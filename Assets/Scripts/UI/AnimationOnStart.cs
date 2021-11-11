using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    public class AnimationOnStart : MonoBehaviour
    {
        [SerializeField] private Vector3 _movePosition;
        [SerializeField] private float _duration;
        [SerializeField] private Ease _easeType = Ease.InSine;
        [SerializeField] private bool _doFade = false;
        [SerializeField] private bool _isLoop = false;

        private Vector3 _startPosition;

        private void Awake()
        {
            _startPosition = transform.position;
            transform.position = _startPosition - _movePosition;
        }

        private void Start()
        {
            var tweener = transform.DOMove(_startPosition, _duration).SetEase(_easeType);

            if (_isLoop)
            {
                tweener.SetLoops(-1);
            }

            if (_doFade)
            {
                if (TryGetComponent<Renderer>(out var renderer))
                {
                    renderer.material.color = SetColorTransparent(renderer.material.color);
                    renderer.material.DOFade(1f, _duration);
                }
                else if (TryGetComponent<Text>(out var text))
                {
                    text.color = SetColorTransparent(text.color);
                    text.DOFade(1f, _duration);
                }
            }
        }

        private Color SetColorTransparent(Color color)
        {
            return new Color(color.r, color.g, color.b, 0f);
        }
    }
}
