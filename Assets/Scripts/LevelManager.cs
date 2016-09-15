using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	public static LevelManager instance;

	public PlayerUIController[] playerUI;

	public Dictionary<PlayerID, BaseControl> playerDict;

	public Dictionary<ControllerInputWrapper.Buttons, float> buttonPitchMap;

	public AudioSource music;

	public bool enableQuarter, enableEighth, enableTriplet;

    /// <summary> Types of rhythms that can be played. </summary>
    public enum RhythmType { None, Quarter, Eighth, Triplet };

	[SerializeField]
	private float tempo;

	[SerializeField]
	private float BPM;

	public float[] keys;
	private int curKey;
	private int beatCounter;
	private float prevBeat = 0f;

    /// <summary>
    /// Initializes the singleton instance of the level manager.
    /// </summary>
    private void Awake() {
        instance = this;
        playerDict = new Dictionary<PlayerID, BaseControl>();
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
	void Start () {
		buttonPitchMap = new Dictionary<ControllerInputWrapper.Buttons, float>();
		buttonPitchMap.Add(ControllerInputWrapper.Buttons.A, 1f);
		buttonPitchMap.Add(ControllerInputWrapper.Buttons.B, Mathf.Pow(2,2/12f));
		buttonPitchMap.Add(ControllerInputWrapper.Buttons.Y, Mathf.Pow(2,4/12f));
		buttonPitchMap.Add(ControllerInputWrapper.Buttons.X, Mathf.Pow(2,7/12f));

		StartCoroutine(ChangeKey());
	}

	private IEnumerator ChangeKey() {
		yield return new WaitUntil( () => beatCounter >= 12);
		beatCounter = 0;
		curKey++;
		curKey %= keys.Length;
		buttonPitchMap[ControllerInputWrapper.Buttons.A] = Mathf.Pow(2,keys[curKey]/12f);
		buttonPitchMap[ControllerInputWrapper.Buttons.B] = Mathf.Pow(2,(keys[curKey]+2)/12f);
		buttonPitchMap[ControllerInputWrapper.Buttons.Y] = Mathf.Pow(2,(keys[curKey]+4)/12f);
		buttonPitchMap[ControllerInputWrapper.Buttons.X] = Mathf.Pow(2,(keys[curKey]+7)/12f);
		StartCoroutine(ChangeKey());
		yield return null;
	}

    /// <summary>
    /// Checks how close the current frame is to the beat.
    /// </summary>
    /// <returns>A percentage of how close the current frame is to the beat.</returns>
    /// <param name="rhythmType">A type of rhythm to constrain the beat to.</param>
    public float PerfectTiming (RhythmType rhythmType = RhythmType.None) {
		float calcTempo = 60f/BPM;

		float val = 2f;
		int samplesInTempo = (int)(music.clip.frequency*calcTempo);
		int samplesPastBeat = music.timeSamples%(samplesInTempo);

		float t = ((float) samplesPastBeat / samplesInTempo);	// how far along we are in this beat (0..1)

		float quarterAccuracy = Mathf.Abs(2 * (t%1) - 1);
		float eighthAccuracy = Mathf.Abs(2 * ((t * 2) % 1) - 1);
		float tripletAccuracy = Mathf.Abs(2 * ((t * 3) % 1) - 1);

		float result = 0f;

		if(enableQuarter) {
			result = quarterAccuracy;
            if (rhythmType == RhythmType.Quarter) {
                return result;
            }
		}
		if(enableEighth) {
            result = Mathf.Max(result,eighthAccuracy);
            if (rhythmType == RhythmType.Eighth) {
                return result;
            }
		}
		if(enableTriplet) {
            result = Mathf.Max(result,tripletAccuracy);
		}

		return result;

	}

	public float BeatValue(float offset) {
		int samplesInTempo = (int)(music.clip.frequency*tempo);
		int samplesPastBeat = (music.timeSamples+(int)(offset*music.clip.frequency))%(samplesInTempo);

		float value = 1f-((float)samplesPastBeat/(float)samplesInTempo);

		if(prevBeat < value) beatCounter++;
		prevBeat = value;

		return value;

	}

	public float Tempo {
		get { return tempo;	}
	}

    /// <summary> The currently enabled rhythms, in order of slowest to fastest. </summary>
    public List<RhythmType> EnabledRhythms {
        get {
            List<RhythmType> rhythmTypes = new List<RhythmType>(3);
            if (enableQuarter) {
                rhythmTypes.Add(RhythmType.Quarter);
            }
            if (enableEighth) {
                rhythmTypes.Add(RhythmType.Eighth);
            }
            if (enableTriplet) {
                rhythmTypes.Add(RhythmType.Triplet);
            }
            return rhythmTypes;
        }
    }
}
