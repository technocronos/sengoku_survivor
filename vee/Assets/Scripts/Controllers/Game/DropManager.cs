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
            var skillId = 0;
            var text = "報酬を選択";
            if (dropId == 0)
            {
                var row = GameManager.Instance.SkillManager.GetSelectableSkills()[0];
                skillId = row["skill_id"];
                text = $"{row["name"]}\n{row["type_name"]}";
            }
            else
            {
                // drop_mstからdropIdに対応するskill_idを取得
                var dropRow = this.skillMst?.Find(i => (int)i["skill_id"] == dropId && (int)i["type"] == 0);
                if (dropRow != null)
                {
                    skillId = dropId;
                    text = $"{dropRow["name"]}\n{dropRow["type_name"]}";
                }
                else
                {
                    // dropIdが見つからない場合、デフォルト処理
                    var row = GameManager.Instance.SkillManager.GetSelectableSkills()[0];
                    skillId = row["skill_id"];
                    text = $"{row["name"]}\n{row["type_name"]}";
                }
            }
            var box = GameObject.Instantiate(this.prefab, pos, Quaternion.identity, this.world);
            box.Initialize(skillId, text);
            
            // 生成直後にコライダーを無効化し、次のフレームで有効化（即取得を防ぐため）
            var collider = box.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
                box.StartCoroutine(EnableColliderNextFrame(collider));
            }
        }

        private IEnumerator EnableColliderNextFrame(Collider2D collider)
        {
            yield return null; // 次のフレームまで待つ
            if (collider != null)
            {
                collider.enabled = true;
            }
        }

        public void DropExp(Vector3 pos, int amount)
        {
            var count = amount;
            var player = FindAnyObjectByType<Player>().gameObject;
            for (int i = 0; i < count; i++)
            {
                var offset = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(-0.3f, 0.3f),
                    0f
                );
                var expPiece = GameObject.Instantiate(expPref, pos + offset, Quaternion.identity, this.world);
                expPiece.Obtain(player);
            }
        }
    }
}
