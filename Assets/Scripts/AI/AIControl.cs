using UnityEngine;

/// <summary>
/// Allows a player to move a bardmage.
/// </summary>
public class AIControl : BaseControl {

    /// <summary> The direction that the bardmage is moving in. </summary>
    [HideInInspector]
    public Vector2 currentDirection = Vector2.zero;

    /// <summary>
    /// Gets the directional input to move the bardmage with.
    /// </summary>
    /// <returns>The directional input to move the bardmage with.</returns>
    protected override Vector2 GetDirectionInput() {
        return currentDirection;
    }

    /// <summary>
    /// Checks if the bardmage turns gradually.
    /// </summary>
    /// <returns>Whether the bardmage turns gradually.</returns>
    protected override bool GetGradualTurn() {
        return true;
    }
}