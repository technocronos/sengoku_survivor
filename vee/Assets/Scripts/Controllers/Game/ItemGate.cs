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
            // ItemGate通過時にスキルを直接付与せず、DropItemのみ生成する
            // (ItemBox取得時にAddSkillが呼ばれるため、ここではAddSkillを呼ばない)
            if (this.dropId > 0)
            {
                SengokuSurvivors.DropManager.Instance.DropItem(position, this.dropId);
            }
            else if (this.skillId > 0)
            {
                // dropIdが設定されていない場合は、直接AddSkillを呼ぶ（旧来の動作を維持）
                GameManager.Instance.AddSkill(this.skillId);
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
