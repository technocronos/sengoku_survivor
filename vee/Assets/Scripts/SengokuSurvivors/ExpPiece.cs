using UnityEngine;
using System.Collections;
using Vs;

namespace SengokuSurvivors
{
    public class ExpPiece : MonoBehaviour
    {
        private DropManager dropManager;
        private const float distance = -0.2f;//2.0f;
        private int add_exp = 1;
        public int GetExpAmount()
        {
            return add_exp;
        }

        public void Setup(GameObject target, DropManager dropManager, Vector3 position)
        {
            this.dropManager = dropManager;
            gameObject.SetActive(true);
            transform.SetPositionAndRotation(position, Quaternion.identity);
            transform.Rotate(Vector3.right, -30f);

            this.StartCoroutine(this.Play(target));
        }

        private IEnumerator Play(GameObject target)
        {
            {
                var dir = Vector3.up;//(target.transform.position - this.transform.position).normalized;
                var pos1 = this.transform.position;
                var pos2 = this.transform.position - dir * distance;
                var elapsed = 0.0f;
                while (elapsed <= 1.0f)
                {
                    this.transform.position = Vector3.Lerp(pos1 - 0.1f * Vector3.up, pos2, elapsed);
                    elapsed += Time.deltaTime * 16;
                    yield return null;
                }
                while (elapsed <= 2.0f)
                {
                    this.transform.position = Vector3.Lerp(pos2, pos1, elapsed - 1f);
                    elapsed += Time.deltaTime * 8;
                    yield return null;
                }
            }
            //{
            //    var pos1 = this.transform.position;
            //    var elapsed = 0.0f;
            //    while (elapsed <= 1.0f)
            //    {
            //        this.transform.position = Vector3.Lerp(pos1, target.transform.position, elapsed);
            //        elapsed += Time.deltaTime * 4;
            //        yield return null;
            //    }
            //}
            
            //this.OnComplete();
            while (true)
            {
                yield return null;
                var pos = Camera.main.WorldToViewportPoint(transform.position);
                if (pos.x > 1f || pos.x < 0 || pos.y > 1 || pos.y < 0) dropManager.DespawnExp(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var player = collision.GetComponent<Vs.Controllers.Game.Player>();
            if (player != null)
            {
                StartCoroutine(CatchRoutine(player));
            }
        }

        private IEnumerator CatchRoutine(Vs.Controllers.Game.Player player)
        {
            {
                var pos1 = this.transform.position;
                var elapsed = 0.0f;
                while (elapsed <= 1.0f)
                {
                    this.transform.position = Vector3.Lerp(pos1, player.transform.position, elapsed);
                    elapsed += Time.deltaTime * 4;
                    yield return null;
                }
                this.OnComplete();
            }
        }

        private void OnComplete()
        {
            //add exp to player
            SoundService.Instance.PlaySe("get_item");
            Vs.Controllers.Game.GameManager.Instance.AddExp(add_exp);
            dropManager.DespawnExp(this);
        }
    }
}