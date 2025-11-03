using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class SkillManager
    {
        private List<JsonObject> skillMst;
        private List<JsonObject> allSkillMst; // 元のskill_mst全体を保持
        private List<Skill> skills = new List<Skill>();

        public void Initialize(List<JsonObject> skillMst)
        {
            this.allSkillMst = skillMst; // 元のskill_mst全体を保存
            var skillIds = new int[] { 901};//1001, 1002, 1004, 1007, 1009 };
            this.skillMst = skillMst.FindAll(row =>
            {
                var category = (int)row["category"];
                var skillId = (int)row["skill_id"];
                return category == 201 || System.Array.Exists(skillIds, i => i == skillId);
            });
        }

        public Skill UpgradeSkill(int skillId)
        {
            // まずskillMstから検索、見つからなければallSkillMstから検索
            var raw = this.skillMst.Find(i => i["skill_id"] == skillId);
            if (raw == null)
            {
                raw = this.allSkillMst.Find(i => i["skill_id"] == skillId);
            }
            if (raw == null)
            {
                UnityEngine.Debug.LogError($"Skill {skillId} not found in skill_mst");
                return null;
            }
            var skill = this.skills.Find(i => i.SkillId == skillId);
            if (skill == null)
            {
                skill = new Skill();
                skill.SkillId = skillId;
                skill.Category = raw["category"];
                this.skills.Add(skill);
            }
            skill.Atk += raw["atk"];
            skill.Speed += raw["speed"];
            skill.CoolTime += raw["cooltime"]; // バグ修正: raw["atk"] → raw["cooltime"]
            skill.LifeTime += raw["lifetime"];
            skill.Projectile += raw["projectile"];
            skill.Count += raw["count"];
            skill.Size += raw["size"];
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
            return this.skillMst
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
