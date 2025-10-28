using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class ItemGate : Item
    {
        [SerializeField]
        private UnityEngine.UI.Text text;

        private int skillId;

        protected override void OnComplete()
        {
            GameManager.Instance.AddSkill(this.skillId);
        }

        public void Initialize(JsonObject raw)
        {
            this.skillId = raw["skill_id"];
            this.text.text = $"{raw["name"]}\n{raw["type_name"]}";
        }
    }
}
