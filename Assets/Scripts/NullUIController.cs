using UnityEngine;

/// <summary>
/// Placeholder UI controller for minions.
/// </summary>
public class NullUIController : PlayerUIController {

    /// <summary>
    /// Overrides UI setup to do nothing.
    /// </summary>
    public override void SetupUI() {
    }

    /// <summary>
    /// Overrides tune progression UI updating to do nothing.
    /// </summary>
    /// <param name="t">Unused.</param>
    public override void TuneProgressed(Tune t) {
    }

    /// <summary>
    /// Overrides tune resetting in the UI to do nothing.
    /// </summary>
    public override void TuneReset() {
    }

    /// <summary>
    /// Overrides health updating to do nothing.
    /// </summary>
    /// <param name="amount">Unused.</param>
    /// <param name="died">Unused.</param>
    public override void UpdateHealth(float amount, bool died) {
    }
}