using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI.InGame
{
    public class GameStateView : MonoBehaviour
    {
        [SerializeField] private Ease _transitionType;
        [Range(0f, 2f)]
        [SerializeField] private float _duration;

        [SerializeField] private Image _sourceImage;
        [SerializeField] private RectTransform _transform;
        [SerializeField] private Sprite _battleEndImage;
        [SerializeField] private Sprite _roundReadyImage;
        [SerializeField] private Sprite _roundFightImage;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();

            _sourceImage.DOFade(0f, 0f);
            _transform.anchoredPosition = Vector2.zero;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            DOTween.Sequence()
                .Join(_transform.DOScale(2f, 0f))
                .Append(_sourceImage.DOFade(1f, _duration))
                .Join(_transform.DOScale(1f, _duration).SetEase(_transitionType))
                .AppendInterval(0.25f)
                .Append(_sourceImage.DOFade(0f, _duration))
                .AppendCallback(() => gameObject.SetActive(false))
                .SetLink(gameObject, LinkBehaviour.CompleteAndKillOnDisable);
        }

        public void Ready()
        {
            StartAnimation(_roundReadyImage);
        }

        public void Fight()
        {
            StartAnimation(_roundFightImage);
        }

        public void End()
        {
            StartAnimation(_battleEndImage);
        }

        private void StartAnimation(Sprite source)
        {
            if (gameObject.activeInHierarchy)
            {
                return;
            }

            _sourceImage.sprite = source;
            gameObject.SetActive(true);
        }
    }
}
