using UnityEngine;
using System.Collections;

public class Tune : MonoBehaviour {

	/// <summary>
	/// The keys that need to be pressed to perform this tune.
	/// </summary>
	public ControllerInputWrapper.Buttons[] tune;

	/// <summary>
	/// What object will be spawned when this tune is complete?
	/// </summary>
	public GameObject spawnObject;

	/// <summary>
	/// How much of the tune has been played?
	/// </summary>
	[HideInInspector]
	public int tuneProgress;

	/// <summary>
	/// Was this tune played perfectly?
	/// </summary>
	private bool perfectTiming = true;

	/// <summary>
	/// The owner transform. This is needed to pass information between the attacker
	/// and the tune spawned object.
	/// </summary>
	public Transform ownerTransform;

    /// <summary> The name of the tune. </summary>
    public string tuneName;

	/// <summary> The description of the tune. </summary>
	public string tuneDescription;

    /// <summary> The percentage threshold for a note being considered on the beat. </summary>
    public const float PERFECT_THRESHOLD = 0.8f;

    /// <summary> The maximum distance away from the enemy for the tune to be able to succeed. Used for AI. </summary>
    [SerializeField]
    [Tooltip("The maximum distance away from the enemy for the tune to be able to succeed. Used for AI.")]
    public float maxDistance = Mathf.Infinity;
    /// <summary> The minimum distance away from the enemy for the tune to be able to succeed. Used for AI. </summary>
    [SerializeField]
    [Tooltip("The minimum distance away from the enemy for the tune to be able to succeed. Used for AI.")]
    public float minDistance = 0;

    /// <summary> Whether the tune is being played by a human player. </summary>
    [HideInInspector]
    public bool isHuman;

	/// <summary>
	/// What should happen when the tune completes?
	/// </summary>
	/// <param name="crit">Was the tune played perfectly?</param>
	public virtual void TuneComplete(bool crit) {	}

	/// <summary>
	/// What is the next button in the tune?
	/// </summary>
	/// <returns>The button.</returns>
	public ControllerInputWrapper.Buttons NextButton() {
        if (tuneProgress >= tune.Length) {
            tuneProgress = tune.Length - 1;
        }
		return tune[tuneProgress];
	}

	/// <summary>
	/// Iterates the tune;
	/// </summary>
	/// <returns><c>true</c>, if tune was finished, <c>false</c> otherwise.</returns>
	public bool IterateTune() {
		tuneProgress++;

        bool perfect = LevelManager.instance.PerfectTiming() >= PERFECT_THRESHOLD;
        if (isHuman) {
            LevelManager.instance.RegisterPerfectNote(perfect);
        }
        if(!perfect) {
			perfectTiming = false;
		}

		if(tuneProgress >= tune.Length) {
			TuneComplete(perfectTiming);
			perfectTiming = true;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Resets the tune.
	/// </summary>
	public void ResetTune() {
		tuneProgress = 0;
		perfectTiming = true;
	}

    /// <summary>
    /// Determines whether the specified <see cref="Tune"/> is equal to the current <see cref="Tune"/>.
    /// </summary>
    /// <param name="other">The <see cref="Tune"/> to compare with the current <see cref="Tune"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="Tune"/> is equal to the current <see cref="Tune"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(Tune other) {
        return other != null && tuneName.Equals(other.tuneName);
    }
}
