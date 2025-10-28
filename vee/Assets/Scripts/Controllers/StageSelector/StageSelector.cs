using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame;

namespace Vs.Controllers.StageSelector
{
    public sealed class StageSelector : Controller
    {
        public sealed class Context : ViewContext
        {
            // nop
        }

        [SerializeField]
        private Transform Content;

        [SerializeField]
        private ListItemStage listItemPrefab;

        private List<JsonObject> list = new List<JsonObject>();

        public override IEnumerator OnViewLoaded(ViewContext viewContext)
        {
            this.list = Api.Goods.Get();
            this.Refresh(this.list);
            yield break;
        }

        private void Refresh(List<JsonObject> list)
        {
            for (var index = 0; index < list.Count; index++)
            {
                var raw = list[index];

                var go = GameObject.Instantiate(this.listItemPrefab, this.Content);
                go.Clicked += this.OnListItemClicked;
                go.Initialize(index);

                var sprite = Resources.Load<Sprite>($"Stages/{raw["stage_id"]}");
                go.SetSprite(sprite);
            }
        }

        private void OnListItemClicked(int index)
        {
        }
    }
}
