using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gets player input for generating notes and attacks.
/// </summary>
public class PlayerBard : BaseBard {

    /// <summary>
    /// Checks if a certain button was pressed.
    /// </summary>
    /// <returns>Whether the button was pressed.</returns>
    /// <param name="button">The button to check for.</param>
    protected override bool GetButtonDown(ControllerInputWrapper.Buttons button) {
        return ControllerManager.instance.GetButtonDown(button, control.player);
    }

    /// <summary>
    /// Checks if a certain button is being pressed.
    /// </summary>
    /// <returns>Whether the button is being pressed.</returns>
    /// <param name="button">The button to check for.</param>
    protected override bool GetButton(ControllerInputWrapper.Buttons button) {
        return ControllerManager.instance.GetButton(button, control.player);
    }

    /// <summary>
    /// Sets whether the bard's tunes are played by a human or an AI.
    /// </summary>
    /// <param name="tune">The tune to set.</param>
    protected override void SetTuneHuman(Tune tune) {
        tune.isHuman = true;
    }

    /// <summary>
    /// Checks if a note was played correctly.
    /// </summary>
    /// <param name="correct">Whether a note was played correctly.</param>
    protected override void RegisterNoteCorrect(bool correct) {
        LevelManager.instance.RegisterCorrectNote(correct);
    }
}
