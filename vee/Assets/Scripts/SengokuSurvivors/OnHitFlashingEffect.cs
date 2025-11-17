using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SengokuSurvivors
{
    public class OnHitFlashingEffect : MonoBehaviour
    {

        private SpriteRenderer spriteRenderer;
        private Coroutine flashingRoutine = null;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void TriggerMaterialChange()
        {
            if (flashingRoutine != null)
            {
                StopCoroutine(flashingRoutine);
            }
            flashingRoutine = StartCoroutine(FlashingRoutine());
        }

        private IEnumerator FlashingRoutine()
        {
            spriteRenderer.material.SetColor("_FlashColor", Color.white);

            float currFlashAmount = 0f;
            float currElapsedTime = 0f;
            float flashTime = 0.25f;
            while (currElapsedTime < flashTime)
            {
                currElapsedTime += Time.deltaTime;
                currFlashAmount = Mathf.Lerp(2f, 0f, currElapsedTime/flashTime);
                spriteRenderer.material.SetFloat("_FlashAmount", currFlashAmount);
                yield return null;
            }
        }
    }
}