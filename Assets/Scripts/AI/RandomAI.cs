using UnityEngine;

namespace Bardmages.AI {
    /// <summary>
    /// Randomly moving AI for testing purposes/easy opponent.
    /// </summary>
    class RandomAI : AIController {

        /// <summary>
        /// Updates the AI's actions.
        /// </summary>
        protected override void UpdateAI() {
            if (!bard.isPlayingTune) {
                bard.StartTune(Random.Range(0, 3), false, enabledRhythms[Random.Range(0, enabledRhythms.Count)]);

                control.currentDirection = new Vector2(-transform.position.x, -transform.position.z);
            }
        }
    }
}