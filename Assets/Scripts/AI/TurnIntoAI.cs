using UnityEngine;
using Bardmages.AI;

/// <summary>
/// Debug script that turns all players into AI.
/// </summary>
class TurnIntoAI : MonoBehaviour {

    /// <summary> The AI prefab to reference from. </summary>
    [SerializeField]
    [Tooltip("The AI prefab to reference from.")]
    private GameObject aiPrefab;

    /// <summary> All players in the scene. </summary>
    private GameObject[] players;
    /// <summary> Whether the otherPlayers arrays in the AI controllers have been reset. </summary>
    private bool resetOtherPlayers;

    /// <summary>
    /// Initializes the object.
    /// </summary>
    private void Awake() {
        PlayerControl[] playerControls = GameObject.FindObjectsOfType<PlayerControl>();
        players = new GameObject[playerControls.Length];
        for (int i = 0; i < playerControls.Length; i++) {
            players[i] = playerControls[i].gameObject;
        }

        float respawnTime = aiPrefab.GetComponent<PlayerLife>().respawnTime;
        Tune[] randomTuneChoices = aiPrefab.GetComponent<AIBard>().randomTuneChoices;

        foreach (GameObject player in players) {
            player.AddComponent<NavMeshAgent>();

            AIControl aiControl = player.AddComponent<AIControl>();
            PlayerControl playerControl = player.GetComponent<PlayerControl>();
            aiControl.player = playerControl.player;
            aiControl.speed = playerControl.speed;

            AIBard aiBard = player.AddComponent<AIBard>();
            PlayerBard playerBard = player.GetComponent<PlayerBard>();
            aiBard.tunes = new Tune[3];
            aiBard.instrumentSound = playerBard.instrumentSound;
            aiBard.buttonPressDelay = playerBard.buttonPressDelay;
            aiBard.volumeOverride = playerBard.volumeOverride;
            aiBard.randomTuneChoices = randomTuneChoices;
            aiBard.randomizeTunes = true;

            player.AddComponent<AdaptiveAI>();

            PlayerLife playerLife = player.GetComponent<PlayerLife>();
            if (playerLife.respawnTime == 0) {
                playerLife.respawnTime = respawnTime;
            }

            Destroy(playerControl);
            Destroy(playerBard);
        }
    }

    /// <summary>
    /// Updates the object.
    /// </summary>
    private void Update() {
        if (!resetOtherPlayers) {
            foreach (GameObject player in players) {
                player.GetComponent<AIController>().FindOtherPlayers();
            }
            resetOtherPlayers = true;
        }
    }
}