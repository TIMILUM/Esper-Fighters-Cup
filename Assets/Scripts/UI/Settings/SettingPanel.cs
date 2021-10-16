using DG.Tweening;
using UnityEngine;

namespace EsperFightersCup.UI.Settings
{
    public abstract class SettingPanel : MonoBehaviour, ISettingPanel
    {
        protected CanvasGroup Group { get; private set; }
        protected RectTransform Transform { get; private set; }

        protected virtual void Awake()
        {
            Group = GetComponent<CanvasGroup>();
            Transform = GetComponent<RectTransform>();

            Group.alpha = 0f;
            Transform.anchoredPosition = Vector2.zero + (Vector2.up * 5f);
        }

        public abstract void Save();

        public virtual void Show()
        {
            // gameObject.SetActive(true);
            Group.interactable = true;

            DOTween.Sequence()
                .Join(Group.DOFade(1f, 0.25f))
                .Join(Transform.DOAnchorPos(Vector2.zero, 0.25f));
        }

        public virtual void Hide()
        {
            Group.interactable = false;

            DOTween.Sequence()
                .Join(Group.DOFade(0f, 0.25f))
                .Join(Transform.DOAnchorPos(Vector2.up * 5f, 0.25f));

            // gameObject.SetActive(false);
        }
    }
}
