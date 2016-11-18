using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bardmages.AI;

/// <summary>
/// Base class for generating notes and attacks.
/// </summary>
public abstract class BaseBard : MonoBehaviour {

	/// <summary>
	/// The tunes this player has equipped.
	/// </summary>
	public Tune[] tunes;

	/// <summary>
	/// The sound their instrument makes.
	/// </summary>
	public AudioClip instrumentSound;

	/// <summary>
	/// What is the minimum time that must pass before this player
	/// is allowed to play the next key in the tune?
	/// </summary>
	public float buttonPressDelay = 0.1f;
	protected float buttonPressDelayTimer;

	/// <summary>
	/// Overrides the volume for this character and ensures that
	/// the instruments are all at an even volume
	/// </summary>
	public float volumeOverride;

	/// <summary>
	/// What tunes is this player currently completing?
	/// </summary>
	private List<Tune> currentTunes;

    /// <summary> The bard's movement controls. </summary>
    protected BaseControl control;
    /// <summary> The bard's health. </summary>
    protected PlayerLife life;

    protected virtual void Start() {
        control = GetComponent<BaseControl>();
        life = GetComponent<PlayerLife>();

        Tune[] tempTunes = new Tune[tunes.Length];
        currentTunes = new List<Tune>();

        for(int i = 0; i < tunes.Length; i++) {
            Tune temp = (Tune)GameObject.Instantiate(tunes[i],Vector3.zero,Quaternion.identity);
            tempTunes[i] = temp;
            tempTunes[i].ownerTransform = transform;
            SetTuneHuman(temp);
            temp.transform.parent = transform;
        }

        tunes = tempTunes;

        if(volumeOverride != 0) GetComponent<AudioSource>().volume = volumeOverride;
    }

    /// <summary>
    /// Sets whether the bard's tunes are played by a human or an AI.
    /// </summary>
    /// <param name="tune">The tune to set.</param>
    protected abstract void SetTuneHuman(Tune tune);

    /// <summary>
    /// Checks for inputs to make notes with.
    /// </summary>
    void Update () {
        if (!life.Alive) {
            return;
        }
        if(buttonPressDelayTimer < 0f) {
            bool soundPlayed = false; //prevents two sounds from being played the same frame
            if(currentTunes.Count > 0) {
                //This makes sure that if we have started a tune, that we only continue iterating that tune
                for(int i = 0; i < currentTunes.Count; i++) {
                    if(GetButtonDown(currentTunes[i].NextButton())) {
                        if(!soundPlayed) {
                            GetComponent<AudioSource>().pitch = LevelManager.instance.buttonPitchMap[currentTunes[i].NextButton()];
                            GetComponent<AudioSource>().PlayOneShot(instrumentSound, volumeOverride);
                            soundPlayed = true;
                        }
                        IterateTune(currentTunes[i]);
                        for (int j = 0; j < currentTunes.Count; j++) {
                            if(j != i && !GetButton(currentTunes[j].NextButton())) {
                                i = 0;
                                currentTunes.Remove(currentTunes[j]);
                                break;
                            }
                        }
                    }
                }
            } else {
                foreach(Tune t in tunes) {
                    if(GetButtonDown(t.NextButton())) {
                        if(!currentTunes.Contains(t)) {
                            currentTunes.Add(t);
                        }
                        if(!soundPlayed) {
                            GetComponent<AudioSource>().pitch = LevelManager.instance.buttonPitchMap[t.NextButton()];
                            GetComponent<AudioSource>().PlayOneShot(instrumentSound, volumeOverride);
                            soundPlayed = true;
                        }
                        IterateTune(t);
                    }
                }
            }
        } else {
            foreach(Tune t in tunes) {
                if(ControllerManager.instance.GetButtonDown(t.NextButton(),control.player)) {
                    StopAllCoroutines();
                    foreach (Tune x in tunes) {
                        x.ResetTune();
                        LevelManager.instance.GetPlayerUI(control.player).TuneReset();
                    }
                    break;
                }
            }
            buttonPressDelayTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Checks if a certain button was pressed.
    /// </summary>
    /// <returns>Whether the button was pressed.</returns>
    /// <param name="button">The button to check for.</param>
    protected abstract bool GetButtonDown(ControllerInputWrapper.Buttons button);

    /// <summary>
    /// Checks if a certain button is being pressed.
    /// </summary>
    /// <returns>Whether the button is being pressed.</returns>
    /// <param name="button">The button to check for.</param>
    protected abstract bool GetButton(ControllerInputWrapper.Buttons button);

    private IEnumerator TuneTimeOut() {
        yield return new WaitForSeconds(LevelManager.instance.Tempo*2f);
        if (currentTunes.Count != 0) {
            RegisterNoteCorrect(false);
        }
        foreach (Tune x in tunes) {
            x.ResetTune();
            LevelManager.instance.GetPlayerUI(control.player).TuneReset();
            currentTunes.Clear();
        }
        yield return null;
    }

    private void IterateTune(Tune t) {
        buttonPressDelayTimer = buttonPressDelay;

        LevelManager.instance.GetPlayerUI(control.player).TuneProgressed(t);

        StopAllCoroutines();
        StartCoroutine(TuneTimeOut());

        if(t.IterateTune()) {
            RegisterNoteCorrect(true);
            foreach (Tune x in tunes) {
                x.ResetTune();
                LevelManager.instance.GetPlayerUI(control.player).TuneReset();
                currentTunes.Clear();
            }
        }
    }

    /// <summary>
    /// Notifies the bard when its tune hits another bard.
    /// </summary>
    /// <param name="tune">The tune that caused the damage.</param>
    /// <param name="weight">A weight for the effectiveness of the tune.</param>
    /// <param name="isDamage">Whether the weight is the amount of damage dealt by the tune.</param>
    public void CreditHit(Tune tune, float weight, bool isDamage = true) {
        AdaptiveAI ai = GetComponent<AdaptiveAI>();
        if (ai != null) {
            ai.RegisterHit(tune, weight);
        }
    }

    /// <summary>
    /// Gets the index of a tune in the bardmage's tune book.
    /// </summary>
    /// <returns>The index of the tune, or -1 if the tune was not found.</returns>
    /// <param name="tune">Tune.</param>
    public int GetTuneIndex(Tune tune) {
        for (int i = 0; i < tunes.Length; i++) {
            if (tunes[i].Equals(tune)) {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Gets a bardmage's tune from its name.
    /// </summary>
    /// <returns>The tune with the given name, or null if the bardmage doesn't have that tune.</returns>
    /// <param name="tuneName">The name of the tune to look for.</param>
    public Tune GetTuneFromName(string tuneName) {
        for (int i = 0; i < tunes.Length; i++) {
            if (tunes[i].tuneName == tuneName) {
                return tunes[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Checks if a note was played correctly.
    /// </summary>
    /// <param name="correct">Whether a note was played correctly.</param>
    protected virtual void RegisterNoteCorrect(bool correct) {
    }
}
