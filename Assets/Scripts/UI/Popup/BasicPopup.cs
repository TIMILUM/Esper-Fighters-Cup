using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EsperFightersCup.UI.Popup
{
    // TODO: 싱글톤으로 PopupManager 만들어서 관리 및 호출
    public class BasicPopup : MonoBehaviour
    {
        [SerializeField] private Text _title;
        [SerializeField] private Text _description;
        [SerializeField] private Button _yesButton;

        public event UnityAction OnYesButtonClicked;

        private CanvasGroup _group;

        private void Awake()
        {
            _group = GetComponent<CanvasGroup>();
            _group.alpha = 0f;
            transform.position = transform.position + (Vector3.up * 5f);

            _title.text = _description.text = string.Empty;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Enter 입력 시 팝업 사라지고 미리 선택되어 있던 버튼이 바로 실행되어서 해당 선택 해제
                EventSystem.current.firstSelectedGameObject = null;
                _yesButton.onClick?.Invoke();
            }
        }

        public void Open(string title, string description)
        {
            _title.text = title;
            _description.text = description;

            _yesButton.onClick.AddListener(() =>
            {
                OnYesButtonClicked?.Invoke();
                Destroy(gameObject);
            });

            DOTween.Sequence()
                .SetLink(gameObject)
                .Join(transform.DOMoveY(transform.position.y - 5f, 0.25f))
                .Join(_group.DOFade(1f, 0.25f));
        }
    }
}
