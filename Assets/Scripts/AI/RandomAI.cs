using UnityEngine;
using System.Collections;

/// <summary>
/// Randomly moving AI for testing purposes/easy opponent.
/// </summary>
public class RandomAI : AIController {

    /// <summary>
    /// Changes any needed settings for the AI.
    /// </summary>
    protected override void InitializeAI() {
        base.InitializeAI();
        bard.timingVariance = 0.1f;
    }

    /// <summary>
    /// Updates the AI's actions.
    /// </summary>
    protected override void UpdateAI() {
        if (!bard.IsPlayingTune()) {
            bard.StartTune(Random.Range(0, 3), enabledRhythms[Random.Range(0, enabledRhythms.Count)]);

            control.currentDirection = new Vector2(-transform.position.x, -transform.position.z);
        }
    }
}
