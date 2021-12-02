using DG.Tweening;
using UnityEngine;

namespace EsperFightersCup.UI.InGame.Skill
{
    public class RandDropRangeUI : SkillUI
    {
        [SerializeField] private GameObject _innerUI;

        private void Start()
        {
            _innerUI.transform.localScale = Vector3.zero;
            _innerUI.transform.DOScale(1f, Duration).SetLink(gameObject);
        }
    }
}
