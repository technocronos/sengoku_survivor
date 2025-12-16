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
        private Treasure prefab_t;
        [SerializeField]
        private ExpPiece expPref;

        private readonly Queue<ExpPiece> expPiecesCache = new Queue<ExpPiece>();
        private readonly Queue<ItemBox> itemBoxCache = new Queue<ItemBox>();
        private readonly Queue<Treasure> TreasureCache = new Queue<Treasure>();

        private List<JsonObject> drop_mst;

        private void Start()
        {
            this.drop_mst = Vs.Backend.MstDatas.Instance.Get("drop_mst");
        }

        public void DropItem(Vector3 pos, int dropId)
        {

            var dropRow = this.drop_mst.Find(row => row["item_id"] == dropId);

            if (dropRow["category"] == 1 || dropRow["category"] == 2) { 
                var text = "報酬を選択";
                if (dropId == 0)//dropId = 0の場合ランダム
                {
                    var row = GameManager.Instance.EquipmentManager.GetSelectableSkills()[0];
                    dropId = row["item_id"];
                    text = $"{row["name"]}";
                }

                var box = (itemBoxCache.Count > 0) ? itemBoxCache.Dequeue() : Instantiate(this.prefab, this.world);
                box.gameObject.SetActive(true);
                box.transform.SetPositionAndRotation(pos, Quaternion.identity);
                box.transform.Rotate(Vector3.right, -30f);
                box.Setup(dropId, text, this);
            }else if(dropRow["category"] == 3)
            {
                var treasure = (TreasureCache.Count > 0) ? TreasureCache.Dequeue() : Instantiate(this.prefab_t, this.world);
                treasure.gameObject.SetActive(true);
                treasure.transform.SetPositionAndRotation(pos, Quaternion.identity);
                treasure.transform.Rotate(Vector3.right, -30f);
                treasure.Setup(dropId, this);
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
