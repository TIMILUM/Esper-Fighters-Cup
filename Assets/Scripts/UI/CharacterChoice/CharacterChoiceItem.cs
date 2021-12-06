using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    [RequireComponent(typeof(Button))]
    public class CharacterChoiceItem : MonoBehaviour
    {
        [SerializeField] private ACharacter.Type _characterType;

        public ACharacter.Type CharacterType => _characterType;
        public Button TargetButton { get; private set; }

        private void Awake()
        {
            TargetButton = GetComponent<Button>();
        }
    }
}
