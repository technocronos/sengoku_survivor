using System.Collections;
using System.Collections.Generic;
using System.Data;
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
        private Dictionary<int, Equipment> skills = new Dictionary<int, Equipment>();
        private readonly Dictionary<int, Sprite> skillSpriteAssets = new Dictionary<int, Sprite>();

        public void Initialize(List<JsonObject> dropMst, List<JsonObject> weaponsMst, List<JsonObject> accessoriesMst)
        {
            this.dropMst = dropMst;
            this.weaponsMst = weaponsMst;
            this.accessoriesMst = accessoriesMst;
        }

        public Equipment UpgradeSkill(int itemId)
        {
            var dropRow = this.dropMst.Find(i => i["item_id"] == itemId);
            if (dropRow == null)
            {
                return null;
            }
            bool hasItem = skills.ContainsKey(itemId);
            int newLevel = 1;
            if (hasItem) 
            { 
                newLevel = skills[itemId].Level + 1;
            }
            ItemCategory category = (ItemCategory)(int) dropRow["category"];//1 - weapon, 2 - accessory
            JsonObject itemRow = null;
            if (category == ItemCategory.Weapon)
            {
                itemRow = weaponsMst.Find(i => i["item_id"] == itemId && i["level"] == newLevel);
            }
            else if (category == ItemCategory.Accessory)
            {
                itemRow = accessoriesMst.Find(i => i["item_id"] == itemId && i["level"] == newLevel);
            }
            if (itemRow == null) return null;



            Equipment item;
            if (!hasItem)//取得していない武器や装備の場合
            {
                if (category == ItemCategory.Weapon)
                {
                    item = new Weapon();
                }
                else if (category == ItemCategory.Accessory) 
                {
                    item = new Accessory();
                }
                else
                {
                    return null;
                }
                item.ItemId = itemId;
                item.ItemIcon = GetSkillSprite(itemId);
                item.Category = category;
                this.skills.Add(itemId, item);
            }
            else
            {
                item = skills[itemId];
            }

            item.Level = itemRow["level"];

            if (item.Category == ItemCategory.Weapon)
            {
                var weapon = item as Weapon;
                weapon.Atk = itemRow["atk"];
                weapon.CoolTime = itemRow["cooltime"] / 1000f;
                weapon.Count = itemRow["count"];
                weapon.KnockbackTime = itemRow["knockback_time"] / 1000f;
                weapon.KnockbackLength = itemRow["knockback_length"] / 1000f;
            }
            else if (item.Category == ItemCategory.Accessory)
            {
                var acc = item as Accessory;
                acc.EffectId = itemRow["effect_id"];
                acc.EffectValue = itemRow["effect_value"];
            }
            else
            {
                return null;
            }

            return item;
        }

        public Dictionary<int, Equipment> GetCurrentSkills()
        {
            return skills;
        }

        public Equipment GetCurrentSkillWithId(int id)
        {
            if (skills.ContainsKey(id))
                return skills[id];
            else
                return null;
        }

        public List<JsonObject> GetAllSkillMst()
        {
            return this.dropMst;
        }

        public List<JsonObject> GetSelectableSkills()
        {
            return GetSelectableSkillsAll().OrderBy(i => System.Guid.NewGuid()).ToList().Take(3).ToList();
        }

        public List<JsonObject> GetSelectableSkillsAll()
        {
            List<JsonObject> list = new List<JsonObject>();
            for(int i = 0; i < dropMst.Count; i++)
            {
                int level = 1;
                int id = dropMst[i]["item_id"];
                ItemCategory category = (ItemCategory)(int) dropMst[i]["category"];
                if (skills.ContainsKey(id))
                    { level = skills[id].Level; }
                List<JsonObject> data;
                if (category == ItemCategory.Weapon)
                {
                    data = weaponsMst;
                }
                else if (category == ItemCategory.Accessory)
                {
                    data = accessoriesMst;
                }
                else
                {
                    continue;
                }

                if (!data.Exists(j => (j["level"] == level + 1) && (j["item_id"] == id)))
                { continue; }

                list.Add(dropMst[i]);
            }
            return list;
        }

        public bool IsBaseSkillObtained(int id)
        {
            return this.skills.ContainsKey(id);
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
