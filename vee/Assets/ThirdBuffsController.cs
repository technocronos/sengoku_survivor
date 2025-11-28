using UNCHAIN.ThirdSdk;
using UnityEngine;
using Vs.Controllers.Game;
using System.Collections.Generic;

namespace SengokuSurvivors
{
    public class ThirdBuffsController : MonoBehaviour
    {
        [SerializeField]
        private ThirdCommentsUi CommentsUi;
        [SerializeField]
        private BuffIcon buffSpeedUp;
        [SerializeField]
        private BuffIcon buffKnockback;
        [SerializeField]
        private Sprite speedUpSprite;
        [SerializeField]
        private Sprite knockbackUpSprite;

        private float knockbackUpDuration = 10f;
        private float speedUpDuration = 10f;

        private readonly Queue<float> speedupBuffs = new Queue<float>();
        private readonly Queue<float> knockbackBuffs = new Queue<float>();

        private void Awake()
        {
            var a = FindAnyObjectByType<ThirdController>();
            if (a != null) a.BuffsController = this;
        }

        public void AddSpeedupBuff()
        {
            speedupBuffs.Enqueue(Time.time);
            buffSpeedUp.UpdateIcon(speedUpSprite, speedupBuffs.Count);
        }

        public void AddKnockbackBuff()
        {
            knockbackBuffs.Enqueue(Time.time);
            buffKnockback.UpdateIcon(knockbackUpSprite, knockbackBuffs.Count);
        }

        private void Update()
        {
            var count1 = speedupBuffs.Count;
            var count2 = knockbackBuffs.Count;
            if (count1 > 0 && Time.time - speedupBuffs.Peek() > speedUpDuration)
            {
                speedupBuffs.Dequeue();
                buffSpeedUp.UpdateIcon(speedUpSprite, count1 - 1);
            }
            if (count2 > 0 && Time.time - knockbackBuffs.Peek() > knockbackUpDuration)
            {
                knockbackBuffs.Dequeue();
                buffKnockback.UpdateIcon(knockbackUpSprite, count2 - 1);
            }
        }
    }
}