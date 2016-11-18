using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

	public static LevelManager instance;

	public bool inMainMenu;

    /// <summary> The player UI controllers in the scene. </summary>
    [SerializeField]
    [Tooltip("The player UI controllers in the scene.")]
	private PlayerUIController[] playerUI;

    /// <summary> The placeholder UI controller for minions. </summary>
    private NullUIController nullUI;

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

    /// <summary> The chance that a note is played perfectly based on history. </summary>
    public float perfectNoteChance {
        get;
        private set;
    }
    /// <summary> The chance that a tune note is played correctly. </summary>
    public float correctNoteChance {
        get;
        private set;
    }
    /// <summary> The growth rate of the note chance. </summary>
    private const float NOTE_CHANCE_GROWTH = 100;

    /// <summary>
    /// Initializes the singleton instance of the level manager.
    /// </summary>
    private void Awake() {
        instance = this;
        playerDict = new Dictionary<PlayerID, BaseControl>();
        perfectNoteChance = 0.75f;
        correctNoteChance = 0.9f;
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

        // Used for minions.
        GameObject nullUIObject = new GameObject();
        nullUI = nullUIObject.AddComponent<NullUIController>();
        nullUIObject.transform.parent = playerUI[0].transform.parent;
        nullUIObject.name = "NullUI";

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
		int samplesPastBeat = (music.timeSamples+(int)offset+(int)(music.clip.frequency))%(samplesInTempo);

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

    /// <summary>
    /// Checks if a player is registered in a 
    /// </summary>
    /// <returns><c>true</c> if this instance has player the specified playerID; otherwise, <c>false</c>.</returns>
    /// <param name="playerID">Player I.</param>
    public bool HasPlayer(PlayerID playerID) {
        return playerID == PlayerID.None || playerDict.ContainsKey(playerID);
    }

    /// <summary>
    /// Gets the player UI corresponding to the given player ID.
    /// </summary>
    /// <param name="player">The player ID to get a player UI for.</param>
    public PlayerUIController GetPlayerUI(PlayerID player) {
        if (player == PlayerID.None) {
            return nullUI;
        } else {
            return LevelManager.instance.playerUI[(int)player - 1];
        }
    }

    /// <summary>
    /// Increments the number of notes that were played.
    /// </summary>
    /// <param name="perfect">Whether the note was played on the beat.</param>
    public void RegisterPerfectNote(bool perfect) {
        float noteChance = perfectNoteChance;
        RegisterNote(ref noteChance, perfect);
        perfectNoteChance = noteChance;
    }

    /// <summary>
    /// Logs a note as correctly or incorrectly played.
    /// </summary>
    /// <param name="correct">Whether the note was correct for a tune.</param>
    public void RegisterCorrectNote(bool correct) {
        float noteChance = correctNoteChance;
        RegisterNote(ref noteChance, correct);
        correctNoteChance = noteChance;
    }

    /// <summary>
    /// Changes a note chance positively or negatively.
    /// </summary>
    /// <param name="noteChance">The note chance variable to change.</param>
    /// <param name="positive">Whether to increase the chance.</param>
    private void RegisterNote(ref float noteChance, bool positive) {
        float difference;
        if (positive) {
            difference = 1;
        } else {
            difference = -1;
        }
        noteChance *= (NOTE_CHANCE_GROWTH + difference) / NOTE_CHANCE_GROWTH;
        noteChance = Mathf.Clamp(noteChance, 0, 1);
    }

    /// <summary>
    /// Gets the bard component of a player from an ID.
    /// </summary>
    /// <returns>The bard component of the specified player.</returns>
    /// <param name="playerID">The ID of the player to get the bard component of.</param>
    public BaseBard GetBardFromID(PlayerID playerID) {
        return playerDict[playerID].GetComponent<BaseBard>();
    }
}
