using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyGame;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public sealed class EquipmentManager
    {
        private List<JsonObject> dropMst;
        private List<JsonObject> weaponsMst;
        private List<JsonObject> accessoriesMst;
        private List<Equipment> skills = new List<Equipment>();
        private readonly Dictionary<int, Sprite> skillSpriteAssets = new Dictionary<int, Sprite>();

        public void Initialize(List<JsonObject> dropMst, List<JsonObject> weaponsMst, List<JsonObject> accessoriesMst)
        {
            this.dropMst = dropMst;
            this.weaponsMst = weaponsMst;
            this.accessoriesMst = accessoriesMst;
        }

        public Equipment UpgradeSkill(int skillId, int newLevel)
        {
            var dropRow = this.dropMst.Find(i => i["item_id"] == skillId);
            if (dropRow == null)
            {
                return null;
            }
            int category = dropRow["category"];//1 - weapon, 2 - accessory
            JsonObject itemRow = null;
            if (category == 1)
            {
                itemRow = weaponsMst.Find(i => i["item_id"] && i["level"] == newLevel);
            }
            else if (category == 2)
            {
                itemRow = accessoriesMst.Find(i => i["item_id"] && i["level"] == newLevel);
            }
            if (itemRow == null) return null;



            var skill = this.skills.Find(i => i.ItemId == skillId);
            if (skill == null)//取得していない武器や装備の場合
            {
                (skill.Category == 
                skill = new Equipment();
                skill.ItemId = skillId;
                skill.ItemIcon = GetSkillSprite(skillId);
                skill.Category = dropRow["category"];                
                skill.Level = 1;
                this.skills.Add(skill);
            }
            else if (!skill.SkillTypes.ContainsKey(type))//取得している武器で、強化を取得していない
            {
                skill.SkillTypes.Add(type, new SkillType() 
                    { Name = $"      {dropRow["type_name"]}", Level = 0,
                    EffectId = dropRow["effect_id"],
                    EffectValue = dropRow["effect_value"]
                });
            }
            skill.SkillTypes[0].Level++;

            skill.Atk += dropRow["atk"];
            skill.Speed += dropRow["speed"];
            skill.CoolTime += dropRow["cooltime"]; // バグ修正: raw["atk"] → raw["cooltime"]
            skill.CoolTimeMulti *= dropRow["cooltime_multi"] / 1000f;
            skill.LifeTime += dropRow["lifetime"];
            skill.Projectile += dropRow["projectile"];
            skill.Count += dropRow["count"];
            skill.Size += dropRow["size"];
            skill.SizeMulti *= dropRow["size_multi"] / 1000f;
            
            if (type == 0)
            {
                skill.KnockbackTime = dropRow["knockback_time"]/1000f;
                skill.KnockbackLength = dropRow["knockback_length"]/1000f;
            }

            return skill;
        }

        public List<Equipment> GetCurrentSkills()
        {
            return this.skills;
        }

        public List<JsonObject> GetAllSkillMst()
        {
            return this.allSkillMst;
        }

        public List<JsonObject> GetSelectableSkills()
        {
            return this.dropMst
                .FindAll(i => this.skills.Exists(j => j.ItemId == i["skill_id"]) 
                ? i["type"] > 0 || i["category"] == 201 : i["type"] == 0)
                .OrderBy(i => System.Guid.NewGuid()).ToList().Take(3).ToList();
        }

        public List<JsonObject> GetSelectableSkillsAll()
        {
            return this.dropMst
                .FindAll(i => this.skills.Exists(j => j.ItemId == i["skill_id"]) ? i["type"] > 0 
                || i["category"] == 201 : i["type"] == 0);
        }

        public bool IsBaseSkillObtained(int id)
        {
            return this.skills.Exists(j => j.ItemId == id);
        }

        public List<JsonObject> GetSelectableSkillsForSkillId(int skillId)
        {
            // ItemBoxから取得したスキルの選択肢を生成
            // 左：現在のスキル（初期武器など）
            // 右：取得したスキル
            var result = new List<JsonObject>();
            
            // 左：現在所持しているスキルの基本（type=0）を取得
            // 最初に所持したスキルを取得（通常は初期武器）
            var currentSkill = this.skills.FirstOrDefault();
            if (currentSkill != null)
            {
                var currentBaseSkill = this.allSkillMst.Find(i => (int)i["skill_id"] == currentSkill.ItemId && (int)i["type"] == 0);
                if (currentBaseSkill != null)
                {
                    result.Add(currentBaseSkill);
                }
            }
            
            // 右：取得したスキルの基本（type=0）を追加
            var newSkill = this.allSkillMst.Find(i => (int)i["skill_id"] == skillId && (int)i["type"] == 0);
            if (newSkill != null)
            {
                result.Add(newSkill);
            }
            
            // 2つだけ返す（3つ目は非表示）
            return result;
        }

        public Sprite GetSkillSprite(int id)
        {
            if (!skillSpriteAssets.ContainsKey(id))
            {
                skillSpriteAssets.Add(id, Resources.Load<Sprite>($"Skills/{id}"));
            }
            return skillSpriteAssets[id];
        }
    }
}
