using UnityEngine;

namespace EsperFightersCup.UI.InGame.Skill
{
    public class ShockwaveRangeUI : SkillUI
    {
        private void Start()
        {
            Debug.Log(Target);
            Destroy(gameObject, Duration);
        }
    }
}
