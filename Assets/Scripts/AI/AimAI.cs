using UnityEngine;

namespace Bardmages.AI {

    /// <summary>
    /// Aims towards the nearest player and shoots.
    /// </summary>
    class AimAI : AIController {

        /// <summary> The radius of the bardmage's collider. </summary>
        private float radius;

        /// <summary> The current deviation of the bardmage's aim angle from straight-on. </summary>
        private float inaccuracyAngle;
        /// <summary> The maximum deviation of the bardmage's aim angle from straight-on. </summary>
        [SerializeField]
        [Tooltip("The maximum deviation of the bardmage's aim angle from straight-on.")]
        private float inaccuracyMax = 20;
        /// <summary> The chance that the AI will not aim correctly. </summary>
        [SerializeField]
        [Tooltip("The chance that the AI will not aim correctly.")]
        private float inaccuracyRate = 0.5f;

        /// <summary>
        /// Changes any needed settings for the AI.
        /// </summary>
        protected override void InitializeAI() {
            radius = GetComponent<CharacterController>().radius;
        }

        /// <summary>
        /// Updates the AI's actions.
        /// </summary>
        protected override void UpdateAI() {
            // Check if the current move has distance constraints.
            GameObject target = GetTarget();
            Vector3 targetPosition = target.transform.position;
            float moveDistance = 0;

            if (!bard.isPlayingTune) {
                bard.StartTune(ChooseTune(), false, enabledRhythms[Random.Range(0, enabledRhythms.Count)]);

                // Sometimes deviate from the correct angle.
                if (Random.Range(0f, 1f) < inaccuracyRate) {
                    inaccuracyAngle = Random.Range(-inaccuracyMax, inaccuracyMax);
                } else {
                    inaccuracyAngle = 0;
                }
            }

            Vector3 targetOffset = targetPosition - transform.position;
            targetOffset = Quaternion.AngleAxis(inaccuracyAngle, Vector3.up) * targetOffset;
            targetPosition = transform.position + targetOffset;

            float targetDistance = control.GetDistance2D(targetPosition);
            float maxDistance = bard.currentTune.maxDistance;
            float minDistance = bard.currentTune.minDistance;
            if (target.GetComponent<BaseControl>() == null) {
                maxDistance = 0.1f;
                minDistance = 0.1f;
            } else {
                // Stay at least a diameter away from the target to avoid pile-up/"dancing".
                minDistance = Mathf.Max(radius * 2, minDistance);
            }
            if (targetDistance > maxDistance) {
                moveDistance = maxDistance;
            } else if (targetDistance < minDistance) {
                moveDistance = minDistance;
            }

            if (moveDistance != 0) {
                // Approach if too far away from the target for the current attack.
                targetOffset.Normalize();
                targetOffset *= moveDistance;
                control.MoveToPosition(targetPosition + targetOffset);
            } else {
                control.FacePosition(targetPosition, true);
				control.MoveToPosition(targetPosition);
            }
        }

        /// <summary>
        /// Chooses a tune to start playing.
        /// </summary>
        /// <returns>The index of the tune to play next.</returns>
        protected virtual int ChooseTune() {
            return Random.Range(0, bard.tunes.Length);
        }
    }
}