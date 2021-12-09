using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class RotateLoopAnimation : MonoBehaviour
    {
        [SerializeField] private Ease _easeType = Ease.OutBack;
        [SerializeField] private float _rotate = 180f;

        private RectTransform _transform;
        private Image _image;

        private void Start()
        {
            _transform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();

            DOTween.Sequence()
                .SetLink(gameObject)
                .Append(_image.DOFade(0f, 0f))
                .AppendInterval(1f)
                .AppendCallback(() => DOTween.Sequence()
                    .SetLink(gameObject))
                    .Append(_transform.DORotate(_transform.rotation.eulerAngles + (Vector3.forward * _rotate), 1f).SetEase(_easeType))
                    .SetLoops(-1, LoopType.Incremental)
                .Join(_image.DOFade(1f, 1f));
        }
    }
}
