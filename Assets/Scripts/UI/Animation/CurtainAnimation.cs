using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace EsperFightersCup
{
    public class CurtainAnimation : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _curtain;
        [Min(0f)]
        [SerializeField] private float _duration;
        [SerializeField] private Ease _fadeInEase;
        [SerializeField] private Ease _fadeOutEase;
        [SerializeField] private bool _showInStart;

        private void Awake()
        {
            _curtain.blocksRaycasts = _showInStart;
            _curtain.alpha = _showInStart ? 1f : 0f;
        }

        public async UniTask FadeIn()
        {
            await DOTween.Sequence()
                .AppendCallback(() => _curtain.alpha = 0f)
                .AppendCallback(() => _curtain.blocksRaycasts = true)
                .Append(_curtain.DOFade(1f, _duration).SetEase(_fadeInEase))
                .AsyncWaitForCompletion();
        }

        public async UniTask FadeOut()
        {
            await DOTween.Sequence()
                .AppendCallback(() => _curtain.alpha = 1f)
                .Append(_curtain.DOFade(0f, _duration).SetEase(_fadeOutEase))
                .AppendCallback(() => _curtain.blocksRaycasts = false)
                .AsyncWaitForCompletion();
        }

        public void ForceShowCurtain()
        {
            _curtain.blocksRaycasts = true;
            _curtain.alpha = 1f;
        }

        public void ForceHideCurtain()
        {
            _curtain.blocksRaycasts = false;
            _curtain.alpha = 0f;
        }
    }
}
