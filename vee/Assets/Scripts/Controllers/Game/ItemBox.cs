using SengokuSurvivors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class ItemBox : Item
    {
        [SerializeField]
        private UnityEngine.UI.Text text;

        private int skillId;
        private int skillType;

        protected override void OnComplete()
        {
            GameManager.Instance.AddSkill(skillId, skillType);
        }

        public void Setup(int skillId, int type, string text, DropManager dropManager)
        {
            this.dropManager = dropManager;
            this.skillId = skillId;
            this.skillType = type;
            this.text.text = text;
        }
    }
}
