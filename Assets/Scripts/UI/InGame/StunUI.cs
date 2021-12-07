using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup
{
    public class StunUI : MonoBehaviour
    {
        [SerializeField]
        private Image _cooldownImage;

        public void SetAmount(float amount)
        {
            _cooldownImage.fillAmount = amount;
        }
    }
}
