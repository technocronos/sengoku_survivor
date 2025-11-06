using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class SkillManager
    {
        private List<JsonObject> dropMst;
        private List<JsonObject> allSkillMst; // 元のdrop_mst全体を保持
        private List<Skill> skills = new List<Skill>();

        public void Initialize(List<JsonObject> skillMst)
        {
            this.allSkillMst = skillMst; // 元のdrop_mst全体を保存
            var skillIds = new int[] { 901};//1001, 1002, 1004, 1007, 1009 };
            this.dropMst = skillMst.FindAll(row =>
            {
                var category = (int)row["category"];
                var skillId = (int)row["skill_id"];
                return category == 201 || System.Array.Exists(skillIds, i => i == skillId);
            });
        }

        public Skill UpgradeSkill(int skillId, int type)
        {
            // まずskillMstから検索、見つからなければallSkillMstから検索
            var row = this.dropMst.Find(i => i["skill_id"] == skillId && i["type"] == type);
            if (row == null)
            {
                row = this.allSkillMst.Find(i => i["skill_id"] == skillId);
            }
            if (row == null)
            {
                UnityEngine.Debug.LogError($"Skill {skillId} not found in drop_mst");
                return null;
            }
            var skill = this.skills.Find(i => i.SkillId == skillId);
            if (skill == null)//取得していない武器やバフの場合
            {
                if (type != 0)
                {
                    return null;//武装を持ってないと強化もできない。
                    //todo: 武器を持ってない場合、強化ｇドロップされない対応
                }
                skill = new Skill();
                skill.SkillId = skillId;
                skill.Category = row["category"];
                skill.SkillTypes.Add(0, new SkillType() { Name = row["name"], Level = 0 });
                this.skills.Add(skill);
            }
            else if (!skill.SkillTypes.ContainsKey(type))//取得している武器で、強化を取得していない
            {
                skill.SkillTypes.Add(type, new SkillType() 
                    { Name = $"      {row["type_name"]}", Level = 0 });
            }
            if (type > 0 || skill.Category == 201) skill.SkillTypes[type].Level++;//強化を取っているのでレベルが+1上がる

            skill.Atk += row["atk"];
            skill.Speed += row["speed"];
            skill.CoolTime += row["cooltime"]; // バグ修正: raw["atk"] → raw["cooltime"]
            skill.CoolTimeMulti *= row["cooltime_multi"];
            skill.LifeTime += row["lifetime"];
            skill.Projectile += row["projectile"];
            skill.Count += row["count"];
            skill.Size += row["size"];
            skill.SizeMulti *= row["size_multi"];
            return skill;
        }

        public List<Skill> GetCurrentSkills()
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
                .FindAll(i => this.skills.Exists(j => j.SkillId == i["skill_id"]) ? i["type"] > 0 : i["type"] == 0)
                .OrderBy(i => System.Guid.NewGuid()).ToList().Take(3).ToList();
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
                var currentBaseSkill = this.allSkillMst.Find(i => (int)i["skill_id"] == currentSkill.SkillId && (int)i["type"] == 0);
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
    }
}
