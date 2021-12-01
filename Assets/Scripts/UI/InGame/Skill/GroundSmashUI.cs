using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup.UI.InGame.Skill
{
    public class GroundSmashUI : SkillUI
    {
        private void Start()
        {
            Debug.Log(Target);
            Destroy(gameObject, Duration);
        }
    }
}
