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
        private int dropId;

        protected override void OnComplete()
        {
            var position = this.transform.position;
            GameManager.Instance.AddSkill(this.skillId);
            if (this.dropId > 0)
            {
                SengokuSurvivors.DropManager.Instance.DropItem(position, this.dropId);
            }
        }

        public void Initialize(JsonObject raw)
        {
            this.skillId = raw["skill_id"];
            this.text.text = $"{raw["name"]}\n{raw["type_name"]}";
        }

        public void SetDropId(int dropId)
        {
            this.dropId = dropId;
        }
    }
}
