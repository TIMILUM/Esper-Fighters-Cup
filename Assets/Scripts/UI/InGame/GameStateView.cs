using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI.InGame
{
    [RequireComponent(typeof(Text))]
    public class GameStateView : MonoBehaviour
    {
        [SerializeField]
        private Text _stateText;
        [SerializeField]
        private RectTransform _transform;

        private void Awake()
        {
            _stateText = GetComponent<Text>();
            _transform = GetComponent<RectTransform>();

            _stateText.DOFade(0f, 0f);
            _transform.DOAnchorPos(Vector2.zero, 0f);
            gameObject.SetActive(false);

            // CoroutineTimer.SetTimerOnce(() => Show("Ready?", Vector2.left * 20f), 3f);
            // CoroutineTimer.SetTimerOnce(() => Show("Fight!", Vector2.left * 20f), 4f);
            // CoroutineTimer.SetTimerOnce(() => Show("K.O", Vector2.up * 20f), 10f);
        }

        public void Show(string text, Vector2 transitionStart)
        {
            _stateText.text = text;
            gameObject.SetActive(false);
            gameObject.SetActive(true);

            DOTween.Sequence()
                .Join(_transform.DOAnchorPos(transitionStart, 0f))
                .Append(_stateText.DOFade(1f, 0.25f))
                .Join(_transform.DOAnchorPos(Vector2.zero, 0.25f).SetEase(Ease.OutExpo))
                .AppendInterval(0.25f)
                .Append(_stateText.DOFade(0f, 0.25f))
                .AppendCallback(() => gameObject.SetActive(false))
                .SetLink(gameObject, LinkBehaviour.CompleteAndKillOnDisable);
        }
    }
}
