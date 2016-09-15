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

}
