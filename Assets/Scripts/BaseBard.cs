using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    protected virtual void Start() {
        control = GetComponent<BaseControl>();

        Tune[] tempTunes = new Tune[tunes.Length];
        currentTunes = new List<Tune>();

        for(int i = 0; i < tunes.Length; i++) {
            Tune temp = (Tune)GameObject.Instantiate(tunes[i],Vector3.zero,Quaternion.identity);
            tempTunes[i] = temp;
            tempTunes[i].ownerTransform = transform;
            temp.transform.parent = transform;
        }

        tunes = tempTunes;

        if(volumeOverride != 0) GetComponent<AudioSource>().volume = volumeOverride;
    }

    /// <summary>
    /// Checks for inputs to make notes with.
    /// </summary>
    void Update () {
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
                        if(!currentTunes.Contains(t)) currentTunes.Add(t);
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
                        LevelManager.instance.playerUI[(int)control.player - 1].TuneReset();
                    }
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
        foreach (Tune x in tunes) {
            x.ResetTune();
            LevelManager.instance.playerUI[(int)control.player - 1].TuneReset();
            currentTunes.Clear();
        }
        yield return null;
    }

    private void IterateTune(Tune t) {
        buttonPressDelayTimer = buttonPressDelay;

        LevelManager.instance.playerUI[(int)control.player - 1].TuneProgressed(t);

        StopAllCoroutines();
        StartCoroutine(TuneTimeOut());

        if(t.IterateTune()) {
            foreach (Tune x in tunes) {
                x.ResetTune();
                LevelManager.instance.playerUI[(int)control.player - 1].TuneReset();
                currentTunes.Clear();
            }
        }
    }
}
