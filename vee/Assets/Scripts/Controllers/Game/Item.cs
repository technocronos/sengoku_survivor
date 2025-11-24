using SengokuSurvivors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vs.Controllers.Game
{
    public class Item : MonoBehaviour
    {
        [SerializeField]
        private float distance = 2.0f;

        [System.NonSerialized]
        public bool isObtained;

        protected DropManager dropManager;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Player>() == null) return;
            SoundService.Instance.PlaySe("get_item");
            this.OnComplete();
            dropManager.DespawnItem(this);
            //this.Obtain(collision.gameObject);
        }

        public void Obtain(GameObject target)
        {
            if (this.isObtained)
            {
                return;
            }
            this.isObtained = true;
            this.StartCoroutine(this.Play(target));
        }

        private IEnumerator Play(GameObject target)
        {
            yield return null;
            // {
            //     var dir = (target.transform.position - this.transform.position).normalized;
            //     var pos1 = this.transform.position;
            //     var pos2 = this.transform.position - dir * this.distance;
            //     var elapsed = 0.0f;
            //     while (elapsed <= 1.0f)
            //     {
            //         this.transform.position = Vector3.Lerp(pos1, pos2, elapsed);
            //         elapsed += Time.deltaTime * 4;
            //         yield return null;
            //     }
            // }
            // {
            //     var pos1 = this.transform.position;
            //     var elapsed = 0.0f;
            //     while (elapsed <= 1.0f)
            //     {
            //         this.transform.position = Vector3.Lerp(pos1, target.transform.position, elapsed);
            //         elapsed += Time.deltaTime * 4;
            //         yield return null;
            //     }
            // }

        }

        protected virtual void OnComplete()
        {
            // nop
        }

        private void Update()
        {
            var pos = Camera.main.WorldToViewportPoint(transform.position);
            if (pos.x > 1.5f || pos.x < -0.5f || pos.y > 1.5f || pos.y < -0.5f) dropManager.DespawnItem(this);
        }
    }
}
