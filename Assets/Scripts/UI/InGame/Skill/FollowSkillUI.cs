namespace EsperFightersCup.UI
{
    public class FollowSkillUI : SkillUI
    {
        private void Update()
        {
            if (!Target)
            {
                return;
            }

            var targetPos = Target.transform.position;
            SetPosition(targetPos);
        }
    }
}
