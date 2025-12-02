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

        private int dropId;

        protected override void OnComplete()
        {
            var position = this.transform.position;
            // ItemGate通過時にスキルを直接付与せず、DropItemのみ生成する
            // (ItemBox取得時にAddSkillが呼ばれるため、ここではAddSkillを呼ばない)
            if (this.dropId > 0)
            {
                SengokuSurvivors.DropManager.Instance.DropItem(position, this.dropId);
            }
        }

        public void Initialize(JsonObject raw)
        {
            this.dropId = raw["drop_id"];
            this.text.text = $"{raw["name"]}";
        }

        public void SetDropId(int dropId)
        {
            this.dropId = dropId;
        }
    }
}
