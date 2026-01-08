using DG.Tweening;
using MyGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

        public const int MAX_DROP_COUNT = 10;

        string text;

        private void Start()
        {
            this.drop_mst = Vs.Backend.MstDatas.Instance.Get("drop_mst");
        }

        public void DropItem(Vector3 pos, int dropId)
        {

            //dropId = 0の場合ランダム
            if (dropId == 0)
            {
                var row = GameManager.Instance.EquipmentManager.GetSelectableSkills()[0];
                dropId = row["item_id"];
                text = $"{row["name"]}";
            }

            var dropRow = this.drop_mst.Find(row => row["item_id"] == dropId);

            if (dropRow["category"] == 1 || dropRow["category"] == 2) { 
                var box = (itemBoxCache.Count > 0) ? itemBoxCache.Dequeue() : Instantiate(this.prefab, this.world);
                box.gameObject.SetActive(true);
                box.transform.SetPositionAndRotation(pos, Quaternion.identity);
                box.transform.Rotate(Vector3.right, -30f);
                box.Setup(dropId, text, this);
            }else if(dropRow["category"] == 3)
            {
                if (GameManager.Instance.onScreenTreasureCount < MAX_DROP_COUNT)
                {
                    var treasure = (TreasureCache.Count > 0) ? TreasureCache.Dequeue() : Instantiate(this.prefab_t, this.world);
                    treasure.gameObject.SetActive(true);
                    treasure.transform.SetPositionAndRotation(pos, Quaternion.identity);
                    treasure.transform.Rotate(Vector3.right, -30f);
                    treasure.Setup(dropId, this);
                }
                else
                {
                    GameManager.Instance.onScreenTreasure.Enqueue(dropId);
                }
            }
        }

        public void DropExp(Vector3 pos, int amount)
        {
            var count = amount / expPref.GetExpAmount();
            var playerobj = FindAnyObjectByType<Vs.Controllers.Game.Player>();

            if (playerobj == null || !playerobj || playerobj.transform == null) return;

            var player = playerobj.gameObject;

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
                itemBox.transform.GetComponent<SpriteRenderer>().enabled = false;
                itemBox.textcanvas.gameObject.SetActive(true);
                itemBox.text.gameObject.SetActive(true);

                int _x = 0;
                int _y = 4;

                // ブレーキをかけながらに飛び出すようにする
                itemBox.textcanvas.GetComponent<RectTransform>().DOAnchorPos(new Vector3(_x, _y, 0), 1.0f).SetEase(Ease.OutCubic).OnComplete(() => {
                    item.gameObject.SetActive(false);
                    itemBoxCache.Enqueue(itemBox);
                });

            }
            else
            {
                Destroy(item.gameObject);
            }
        }

        public Vector3 getRandumPosition()
        {
            var px = GameManager.Instance.Player.transform.position.x + Random.Range(-3.0f, 3.0f);
            var py = GameManager.Instance.Player.transform.position.y + Random.Range(1.0f, 5.0f);

            // ステージの可動範囲内に補正
            px = Mathf.Clamp(px, Player.stageMinX, Player.stageMaxX);

            var position = new Vector3(px, py, 0);

            return position;
        }

        public void DespawnTreasure(Item item)
        {
            var Treasure = item as Treasure;
            if (Treasure != null)
            {
                item.gameObject.SetActive(false);
                TreasureCache.Enqueue(Treasure);
            }
            else
            {
                Destroy(item.gameObject);
            }


            //キューにある空箱をドロップする
            if (GameManager.Instance.onScreenTreasureCount < MAX_DROP_COUNT && GameManager.Instance.onScreenTreasure.Count > 0)
            {
                var item_id = GameManager.Instance.onScreenTreasure.Dequeue();
                Vector3 position = getRandumPosition();

                DropItem(position, item_id);
            }
        }
    }
}
