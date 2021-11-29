namespace EsperFightersCup.UI.InGame.Skill
{
    public class BasicSkillUI : SkillUI
    {
        private void Start()
        {
            if (Duration > 0f)
            {
                Destroy(gameObject, Duration);
            }
        }
    }
}
