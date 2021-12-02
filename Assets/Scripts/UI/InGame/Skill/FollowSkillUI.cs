using UnityEngine;

namespace EsperFightersCup.UI.InGame.Skill
{
    public class FollowSkillUI : SkillUI
    {
        private void Update()
        {
            if (Target == null)
            {
                return;
            }

            var pos = transform.position;
            var targetPos = Target.transform.position;
            transform.position = new Vector3(targetPos.x, pos.y, targetPos.z);
        }
    }
}
