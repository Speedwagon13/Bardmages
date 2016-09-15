using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls AI bardmages.
/// </summary>
public abstract class AIController : MonoBehaviour {

    /// <summary> The AI component for generating tunes. </summary>
    protected AIBard bard;
    /// <summary> The AI component for moving. </summary>
    protected AIControl control;

    /// <summary> The rhythms that are recognized for perfect attacks. </summary>
    protected List<LevelManager.RhythmType> enabledRhythms;

    /// <summary>
    /// Registers the AI in the level.
    /// </summary>
	private void Start() {
        bard = GetComponent<AIBard>();
        control = GetComponent<AIControl>();
        enabledRhythms = LevelManager.instance.EnabledRhythms;
        LevelControllerManager.instance.AddPlayer(control.player, control);
        InitializeAI();
	}

    /// <summary>
    /// Changes any needed settings for the AI.
    /// </summary>
    protected virtual void InitializeAI() {
    }

    /// <summary>
    /// Updates the controller.
    /// </summary>
    private void Update() {
        bard.UpdateTune();
        UpdateAI();
    }

    /// <summary>
    /// Updates the AI's actions.
    /// </summary>
    protected abstract void UpdateAI();
}
