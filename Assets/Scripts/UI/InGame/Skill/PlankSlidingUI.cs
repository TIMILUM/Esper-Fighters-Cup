namespace EsperFightersCup.UI.InGame.Skill
{
    public class PlankSlidingUI : SkillUI
    {
        private void Update()
        {
            if (Target == null)
            {
                return;
            }

            var targetPos = Target.transform.position;
            var targetForword = Target.transform.forward * 2.0f;

            transform.SetPositionAndRotation(targetPos + targetForword, Target.transform.rotation);

            if (!Target.BuffController.ActiveBuffs.Exists(BuffObject.Type.Sliding))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
