using UnityEngine;

namespace Bardmages.AI {

    /// <summary>
    /// Adapts to how well the player is doing and which attacks are doing better.
    /// </summary>
    class AdaptiveAI : AimAI {

        /// <summary> Weights for how effective each tune has been. </summary>
        private float[] tuneWeights;
        /// <summary> The sum of all tune weights. </summary>
        private float totalTuneWeights;

        /// <summary>
        /// Changes any needed settings for the AI.
        /// </summary>
        protected override void InitializeAI() {
            base.InitializeAI();
            tuneWeights = new float[bard.tunes.Length];
            for (int i = 0; i < tuneWeights.Length; i++) {
                AddTuneWeight(i, 10);
            }
        }

        /// <summary>
        /// Updates the AI's actions.
        /// </summary>
        protected override void UpdateAI() {
            bard.timingAccuracy = LevelManager.instance.perfectNoteChance;
            bard.noteAccuracy = LevelManager.instance.correctNoteChance;
            base.UpdateAI();
        }

        /// <summary>
        /// Chooses a tune to start playing.
        /// </summary>
        /// <returns>The index of the tune to play next.</returns>
        protected override int ChooseTune() {
            float random = Random.Range(0, totalTuneWeights);
            float counter = 0;
            for (int i = 0; i < tuneWeights.Length - 1; i++) {
                counter += tuneWeights[i];
                if (random < counter) {
                    return i;
                }
            }
            return tuneWeights.Length - 1;
        }

        /// <summary>
        /// Adds to (or subtracts from) a particular tune's weight.
        /// </summary>
        /// <param name="index">The index of the tune to modify.</param>
        /// <param name="weight">The weight to add to the tune's weight.</param>
        public void AddTuneWeight(int index, float weight) {
            weight = Mathf.Max(weight, -tuneWeights[index] + 1);
            tuneWeights[index] += weight;
            totalTuneWeights += weight;
        }

        /// <summary>
        /// Lowers the weight of a tune if damage is taken during the tune.
        /// </summary>
        /// <param name="damage">The amount of damage that was taken.</param>
        public void TakeDamage(float damage) {
            AddTuneWeight(bard.currentTuneIndex, -damage);
        }

        /// <summary>
        /// Raises the weight of a tune when it hits an opponent.
        /// </summary>
        /// <param name="tune">The tune that hit an opponent.</param>
        /// <param name="weight">The weight to raise the tune by.</param>
        public void RegisterHit(Tune tune, float weight) {
            int tuneIndex = bard.GetTuneIndex(tune);
            if (tuneIndex != -1) {
                AddTuneWeight(tuneIndex, weight);
            }
        }
    }
}