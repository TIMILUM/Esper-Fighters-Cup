using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup.UI.InGame.Skill
{
    public class PlankSlidingUI : SkillUI
    {

        

        private void Update()
        {
            if (Target.photonView.IsMine)
            {
                var TargetPos = Target.transform.position;
                var TargetForword = Target.transform.forward;
                var LookDir = Quaternion.LookRotation(TargetPos, TargetPos + TargetForword);

                transform.SetPositionAndRotation
                    (TargetPos + TargetForword, LookDir);

                if (!Target.BuffController.ActiveBuffs.Exists(BuffObject.Type.Sliding))
                {
                    Destroy(gameObject);
                }
            }


        }
    }
}
