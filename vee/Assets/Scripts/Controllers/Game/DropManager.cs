using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;
using Vs.Controllers.Game;

namespace SengokuSurvivors
{
    public sealed class DropManager : SingletonMonoBehaviour<DropManager>
    {
        [SerializeField]
        private Transform world;

        [SerializeField]
        private ItemBox prefab;
        [SerializeField]
        private ExpPiece expPref;

        private List<JsonObject> skillMst;

        private void Start()
        {
            this.skillMst = Vs.Backend.MstDatas.Instance.Get("drop_mst");
        }

        public void DropItem(Vector3 pos, int dropId)
        {
            //todo: 今は古いSpawnSkillのまま、DropItemを実装する必要がある
            var skillId = 0;
            var text = "報酬を選択";
            if (dropId == 0)
            {
                var row = GameManager.Instance.SkillManager.GetSelectableSkills()[0];
                skillId = row["skill_id"];
                text = $"{row["name"]}\n{row["type_name"]}";
            }
            var box = GameObject.Instantiate(this.prefab, pos, Quaternion.identity, this.world);
            box.Initialize(skillId, text);
        }

        public void DropExp(Vector3 pos, int amount)
        {
            var expPiece = GameObject.Instantiate(expPref, pos, Quaternion.identity, this.world);
            expPiece.Obtain(FindAnyObjectByType<Player>().gameObject);
        }
    }
}
