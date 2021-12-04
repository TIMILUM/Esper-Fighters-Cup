using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI.InGame
{
    public class GameStateView : MonoBehaviour
    {
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
            DOTween.Sequence()
                .Join(_transform.DOAnchorPos(Vector2.left * 20f, 0f))
                .Append(_sourceImage.DOFade(1f, 0.25f))
                .Join(_transform.DOAnchorPos(Vector2.zero, 0.25f).SetEase(Ease.OutExpo))
                .AppendInterval(0.25f)
                .Append(_sourceImage.DOFade(0f, 0.25f))
                .AppendCallback(() => gameObject.SetActive(false))
                .SetLink(gameObject, LinkBehaviour.CompleteAndKillOnDisable);
        }
    }
}
