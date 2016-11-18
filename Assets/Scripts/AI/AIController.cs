using UnityEngine;
using System.Collections.Generic;

namespace Bardmages.AI {
    /// <summary>
    /// Controls AI bardmages.
    /// </summary>
    abstract class AIController : MonoBehaviour {

        /// <summary> The AI component for generating tunes. </summary>
        protected AIBard bard;
        /// <summary> The AI component for moving. </summary>
        protected AIControl control;
        /// <summary> The health component of the bardmage. </summary>
        protected PlayerLife life;

        /// <summary> The rhythms that are recognized for perfect attacks. </summary>
        protected List<LevelManager.RhythmType> enabledRhythms;

        /// <summary> All players in the scene. </summary>
        protected List<BaseControl> otherPlayers;

        /// <summary> The hill in the scene, if any. </summary>
        protected KingofHill hill;

        /// <summary> Whether the AI is a minion. </summary>
        protected bool isMinion;

        /// <summary>
        /// Generates random tunes for the bard if needed.
        /// </summary>
        private void Awake() {
            bard = GetComponent<AIBard>();
            bard.RandomizeTunes();
        }

        /// <summary>
        /// Registers the AI in the level and finds the other players.
        /// </summary>
    	private void Start() {
            control = GetComponent<AIControl>();
            life = GetComponent<PlayerLife>();
            enabledRhythms = LevelManager.instance.EnabledRhythms;
            if (!LevelManager.instance.HasPlayer(control.player)) {
                LevelControllerManager.instance.AddPlayer(control.player, control);
            }
            bard.timingAccuracy = 0.9f;

            FindOtherPlayers();

            InitializeAI();
    	}

        /// <summary>
        /// Registers the other players in the scene.
        /// </summary>
        public void FindOtherPlayers() {
            MinionTuneSpawn minion = GetComponent<MinionTuneSpawn>();
            PlayerID selfID;
            if (minion != null) {
                selfID = minion.owner;
                isMinion = true;
            } else {
                selfID = control.player;
            }

            otherPlayers = new List<BaseControl>();
            BaseControl[] otherPlayersArray;
            if (Assets.Scripts.Data.RoundHandler.Instance != null)
                otherPlayersArray = Assets.Scripts.Data.RoundHandler.Instance.Control();
            else
                otherPlayersArray = FindObjectsOfType<BaseControl>();
            foreach (BaseControl otherPlayer in otherPlayersArray) {
                if (otherPlayer.player != selfID && otherPlayer.GetComponent<MinionTuneSpawn>() == null) {
                    otherPlayers.Add(otherPlayer);
                }
            }

            hill = FindObjectOfType<KingofHill>();
        }

        /// <summary>
        /// Changes any needed settings for the AI.
        /// </summary>
        protected virtual void InitializeAI() {
        }

        /// <summary>
        /// Updates the controller.
        /// </summary>
        private void Update() {
            if (life.Alive) {
                bard.UpdateTune();
                control.UpdateControl();
                UpdateAI();
            }
        }

        /// <summary>
        /// Updates the AI's actions.
        /// </summary>
        protected abstract void UpdateAI();

        /// <summary>
        /// Gets the closest player to the AI by distance.
        /// </summary>
        /// <returns>The closest player to the AI.</returns>
        protected BaseControl GetClosestPlayer() {
            BaseControl closestPlayer = null;
            float closestDistance = Mathf.Infinity;
            foreach (BaseControl player in otherPlayers) {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < closestDistance) {
                    closestPlayer = player;
                    closestDistance = distance;
                }
            }
            return closestPlayer;
        }

        /// <summary>
        /// Gets the object that the AI should target.
        /// </summary>
        /// <returns>The object that the AI should target.</returns>
        protected GameObject GetTarget() {
            if (hill != null && !isMinion) {
                if (hill.king == null) {
                    return hill.gameObject;
                } else if (hill.king.gameObject != gameObject) {
                    return hill.king.gameObject;
                }
            }
            return GetClosestPlayer().gameObject;
        }
    }
}