using UnityEngine;

namespace SengokuSurvivors {
    public class SlashAnimationEvents : MonoBehaviour
    {
        public void SlashEnded()
        {
            GetComponentInParent<SlashController>().isAnimationPlaying = false;
        }
    }
}