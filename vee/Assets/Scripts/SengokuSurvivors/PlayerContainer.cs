using UnityEngine;

namespace SengokuSurvivors {
    public class PlayerContainer : MonoBehaviour
    {
        private Vs.Controllers.Game.Player player;

        private void Awake()
        {
            player = GetComponentInChildren<Vs.Controllers.Game.Player>();
        }

        private void Update()
        {
            var position = transform.localPosition;
            int calcedSpeed = player.GetPlayerSpeedInt();
            position.y += 0.5f * Time.deltaTime; //calcedSpeed / 1000.0f * Time.deltaTime;
            this.transform.localPosition = position;
        }
    }
}