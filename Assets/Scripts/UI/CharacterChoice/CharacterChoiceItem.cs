using EsperFightersCup.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    [RequireComponent(typeof(Button))]
    public class CharacterChoiceItem : MonoBehaviour
    {
        [SerializeField] private ACharacter.Type _characterType;

        private Button _targetButton;

        public ACharacter.Type CharacterType => _characterType;

        private void Awake()
        {
            _targetButton = GetComponent<Button>();
            _targetButton.onClick.AddListener(Choose);
        }

        private void Choose()
        {
            CharacterChoiceSystem.Instance.ChooseCharacter = _characterType;
        }
    }
}
