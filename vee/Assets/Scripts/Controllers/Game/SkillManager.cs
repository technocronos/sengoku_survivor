using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class SkillManager
    {
        private List<JsonObject> skillMst;
        private List<Skill> skills = new List<Skill>();

        public void Initialize(List<JsonObject> skillMst)
        {
            var skillIds = new int[] { 1001, 1002, 1004, 1007, 1009 };
            this.skillMst = skillMst.FindAll(raw =>
            {
                var type = (int)raw["type"];
                var skillId = (int)raw["skill_id"];
                return type == 201 || System.Array.Exists(skillIds, i => i == skillId);
            });
        }

        public Skill UpgradeSkill(int skillId)
        {
            var raw = this.skillMst.Find(i => i["skill_id"] == skillId);
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
            skill.CoolTime += raw["atk"];
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

        public List<JsonObject> GetSelectableSkills()
        {
            return this.skillMst
                .FindAll(i => this.skills.Exists(j => j.SkillId == i["skill_id"]) ? i["type"] > 0 : i["type"] == 0)
                .OrderBy(i => System.Guid.NewGuid()).ToList().Take(3).ToList();
        }
    }
}
