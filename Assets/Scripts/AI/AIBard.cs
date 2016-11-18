using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data;

namespace Bardmages.AI {
    /// <summary>
    /// Hooks AI decisions into generating notes and attacks.
    /// </summary>
    class AIBard : BaseBard {

        /// <summary> The tune button currently being pressed. </summary>
        private ControllerInputWrapper.Buttons pressedButton;
        /// <summary> The rhythm that the current tune is being played at. </summary>
        private LevelManager.RhythmType rhythmType;

        /// <summary> The current tune that the bardmage is playing. </summary>
        internal Tune currentTune;
        /// <summary> The index of the current tune that the bardmage is playing. </summary>
        internal int currentTuneIndex;
        /// <summary> The progress of the bardmage through its current tune. </summary>
        private int tuneProgress;
        /// <summary> Whether the bardmage is currently playing a tune. </summary>
        internal bool isPlayingTune {
            get { return currentTune != null; }
        }
        internal bool isTuneAlmostFinished {
            get { return currentTune != null && tuneProgress >= currentTune.tune.Length - 1; }
        }

        /// <summary> The minimum time delay between notes being played. </summary>
        private float noteDelay;

        /// <summary> The bardmage's accuracy for playing on the beat. </summary>
        [HideInInspector]
        internal float timingAccuracy;
        /// <summary> The percentage threshold of beat correctness for which the bardmage will make a note at. </summary>
        private float modifiedThreshold = Tune.PERFECT_THRESHOLD;

        /// <summary> The bardmage's accuracy for playing the correct notes. </summary>
        internal float noteAccuracy = 1;

        /// <summary>
        /// The possible tunes that the AI can randomly bring into battle.
        /// </summary>
        [Tooltip("The possible tunes that the AI can randomly bring into battle.")]
        public Tune[] randomTuneChoices;
        /// <summary> Whether to randomize the bardmage's tunes. </summary>
        [Tooltip("Whether to randomize the bardmage's tunes.")]
        public bool randomizeTunes;

        /// <summary>
        /// Calculates the time delay between notes.
        /// </summary>
        protected override void Start() {
            base.Start();
            noteDelay = LevelManager.instance.Tempo / 3;
        }

        /// <summary>
        /// Sets whether the bard's tunes are played by a human or an AI.
        /// </summary>
        /// <param name="tune">The tune to set.</param>
        protected override void SetTuneHuman(Tune tune) {
            tune.isHuman = false;
        }

        /// <summary>
        /// Picks random tunes for the bardmage to use during the battle.
        /// </summary>
        internal void RandomizeTunes() {
            if (randomizeTunes && randomTuneChoices.Length > 0) {
                List<int> indexChoices = new List<int>();

                for (int i = 0; i < randomTuneChoices.Length; i++) {
                    indexChoices.Add(i);
                }
                if (indexChoices.Count < tunes.Length) {
                    for (int i = 0; i < tunes.Length - indexChoices.Count; i++) {
                        indexChoices.Add(i);
                    }
                }

                PlayerID playerID = GetComponent<BaseControl>().player;
                for (int i = 0; i < tunes.Length; i++) {
                    int randomIndex = Random.Range(0, indexChoices.Count);
                    Tune randomTune = randomTuneChoices[indexChoices[randomIndex]];
                    tunes[i] = randomTune;
                    Data.Instance.AddTuneToPlayer(playerID, randomTune, i);
                    indexChoices.RemoveAt(randomIndex);
                }
            }
        }

        /// <summary>
        /// Checks if a certain button was pressed.
        /// </summary>
        /// <returns>Whether the button was pressed.</returns>
        /// <param name="button">The button to check for.</param>
        protected override bool GetButtonDown(ControllerInputWrapper.Buttons button) {
            return pressedButton == button;
        }

        /// <summary>
        /// Checks if a certain button is being pressed.
        /// </summary>
        /// <returns>Whether the button is being pressed.</returns>
        /// <param name="button">The button to check for.</param>
        protected override bool GetButton(ControllerInputWrapper.Buttons button) {
            return GetButtonDown(button);
        }

        /// <summary>
        /// Updates the bard's progress through the current tune.
        /// </summary>
        internal void UpdateTune() {
            // Reset the current button.
            pressedButton = ControllerInputWrapper.Buttons.Start;

            // Plays the tune on the beat.
            if (currentTune != null && buttonPressDelayTimer < 0f && LevelManager.instance.PerfectTiming(rhythmType) >= modifiedThreshold) {
                if (Random.Range(0f, 1f) < noteAccuracy) {
                    pressedButton = currentTune.tune[tuneProgress];
                    if (++tuneProgress >= currentTune.tune.Length) {
                        tuneProgress = 0;
                        currentTune = null;
                    }
                }
                buttonPressDelay = Mathf.Max(buttonPressDelayTimer, noteDelay);

                modifiedThreshold = Tune.PERFECT_THRESHOLD;
                if (Random.Range(0.0f, 1.0f) > timingAccuracy) {
                    modifiedThreshold -= 0.2f;
                }
            }
        }

        /// <summary>
        /// Starts executing a tune attack.
        /// </summary>
        /// <param name="tuneIndex">The index of the tune attack.</param>
        /// <param name="overrideCurrent">Whether to override the current action if already executing one.</param>
        /// <param name="rhythmType">The rhythm to play the tune at.</param>
        internal void StartTune(int tuneIndex, bool overrideCurrent = false, LevelManager.RhythmType rhythmType = LevelManager.RhythmType.None) {
            if (overrideCurrent || !isPlayingTune) {
                currentTune = tunes[tuneIndex];
                currentTuneIndex = tuneIndex;
                this.rhythmType = rhythmType;
            }
        }
    }
}
