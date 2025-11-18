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

        private readonly Queue<ExpPiece> expPiecesCache = new Queue<ExpPiece>();
        private readonly Queue<ItemBox> itemBoxCache = new Queue<ItemBox>();

        private List<JsonObject> skillMst;

        private void Start()
        {
            this.skillMst = Vs.Backend.MstDatas.Instance.Get("drop_mst");
        }

        public void DropItem(Vector3 pos, int dropId)
        {
            var skillId = 0;
            var type = 0;
            var text = "報酬を選択";
            if (dropId == 0)//dropId = 0の場合、ランダムのアップグレードをアイテムとして用意
            {
                var row = GameManager.Instance.SkillManager.GetSelectableSkills()[0];
                skillId = row["skill_id"];
                type = row["type"];
                text = $"{row["name"]}\n{row["type_name"]}";
            }
            //else
            //{
            //    // drop_mstからdropIdに対応するskill_idを取得
            //    var dropRow = this.skillMst?.Find(i => (int)i["skill_id"] == dropId && (int)i["type"] == 0);
            //    if (dropRow != null)
            //    {
            //        skillId = dropId;
            //        text = $"{dropRow["name"]}\n{dropRow["type_name"]}";
            //    }
            //    else
            //    {
            //        // dropIdが見つからない場合、デフォルト処理
            //        var row = GameManager.Instance.SkillManager.GetSelectableSkills()[0];
            //        skillId = row["skill_id"];
            //        text = $"{row["name"]}\n{row["type_name"]}";
            //    }
            //}
            var box = (itemBoxCache.Count > 0) ? itemBoxCache.Dequeue() : Instantiate(this.prefab, this.world);
            box.transform.SetPositionAndRotation(pos, Quaternion.identity);
            box.transform.Rotate(Vector3.right, -30f);
            box.Setup(skillId, type, text, this);
            
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
            var count = amount / expPref.GetExpAmount();
            var player = FindAnyObjectByType<Player>().gameObject;
            for (int i = 0; i < count; i++)
            {
                var offset = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(-0.3f, 0.3f),
                    0f
                );
                var expPiece = (expPiecesCache.Count > 0) ? expPiecesCache.Dequeue() : Instantiate(expPref, world);
                expPiece.Setup(player, this, pos + offset);
            }
        }

        public void DespawnExp(ExpPiece exp)
        {
            expPiecesCache.Enqueue(exp);
            exp.gameObject.SetActive(false); 
        }

        public void DespawnItem(Item item)
        {
            var itemBox = item as ItemBox;
            if (itemBox != null)
            {
                item.gameObject.SetActive(false);
                itemBoxCache.Enqueue(itemBox);
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
    }
}
