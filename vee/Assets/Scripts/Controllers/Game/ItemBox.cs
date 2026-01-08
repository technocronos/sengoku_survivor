using SengokuSurvivors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class ItemBox : Item
    {
        public UnityEngine.UI.Text text;
        public Transform textcanvas;

        private int skillId;

        protected override void OnComplete()
        {
            GameManager.Instance.AddSkill(skillId);
        }

        public void Setup(int skillId, string text, DropManager dropManager)
        {
            this.dropManager = dropManager;
            this.skillId = skillId;
            this.text.text = text;
            this.text.gameObject.SetActive(false);
        }
    }
}
