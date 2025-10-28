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

        protected override void OnComplete()
        {
            GameManager.Instance.AddSkill(this.skillId);
        }

        public void Initialize(int skillId, string text)
        {
            this.skillId = skillId;
            this.text.text = text;
        }
    }
}
