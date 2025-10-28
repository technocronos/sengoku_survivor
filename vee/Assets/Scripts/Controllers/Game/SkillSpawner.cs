using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.Game
{
    public sealed class SkillSpawner : SingletonMonoBehaviour<SkillSpawner>
    {
        [SerializeField]
        private Transform world;

        [SerializeField]
        private ItemBox prefab;

        private List<JsonObject> skillMst;

        private void Start()
        {
            this.skillMst = Backend.MstDatas.Instance.Get("skill_mst");
        }

        public void Spawn(Vector3 pos, int dropId)
        {
            var skillId = 0;
            var text = "報酬を選択";
            if (dropId == 0)
            {
                var raw = GameManager.Instance.SkillManager.GetSelectableSkills()[0];
                skillId = raw["skill_id"];
                text = $"{raw["name"]}\n{raw["type_name"]}";
            }
            var box = GameObject.Instantiate(this.prefab, pos, Quaternion.identity, this.world);
            box.Initialize(skillId, text);
        }
    }
}
