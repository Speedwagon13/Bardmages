using UnityEngine;

namespace Bardmages.AI {
    /// <summary>
    /// Modified AI for main menu behavior
    /// </summary>
    class MainMenuAI : AimAI {

		public bool leaveArena;

		private float startFighting = 5f;

        /// <summary>
        /// Updates the AI's actions.
        /// </summary>
        protected override void UpdateAI() {
            if(startFighting > 0f) {
                startFighting -= Time.deltaTime;
            } else if (leaveArena) {
                control.currentDirection = new Vector2(0f, -1f);
            } else {
                base.UpdateAI();
            }
        }
    }
}