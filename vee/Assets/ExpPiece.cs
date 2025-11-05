using UnityEngine;
using System.Collections;
using Vs;

namespace SengokuSurvivors
{
    public class ExpPiece : MonoBehaviour
    {
        private bool isObtained = false;
        private const float distance = 2.0f;
        private int add_exp = 1;
        public int GetExpAmount()
        {
            return add_exp;
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
            {
                var dir = (target.transform.position - this.transform.position).normalized;
                var pos1 = this.transform.position;
                var pos2 = this.transform.position - dir * distance;
                var elapsed = 0.0f;
                while (elapsed <= 1.0f)
                {
                    this.transform.position = Vector3.Lerp(pos1, pos2, elapsed);
                    elapsed += Time.deltaTime * 4;
                    yield return null;
                }
            }
            {
                var pos1 = this.transform.position;
                var elapsed = 0.0f;
                while (elapsed <= 1.0f)
                {
                    this.transform.position = Vector3.Lerp(pos1, target.transform.position, elapsed);
                    elapsed += Time.deltaTime * 4;
                    yield return null;
                }
            }
            SoundService.Instance.PlaySe("get_item");
            this.OnComplete();
            GameObject.Destroy(this.gameObject);
            yield break;
        }

        private void OnComplete()
        {
            //add exp to player
            Vs.Controllers.Game.GameManager.Instance.AddExp(add_exp);
        }
    }
}