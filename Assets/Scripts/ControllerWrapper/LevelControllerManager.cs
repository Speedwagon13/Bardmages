using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelControllerManager : MonoBehaviour {

	public static LevelControllerManager instance;

	public Sprite XBOX_A, PS4_A, KEY_A, XBOX_B, PS4_B, KEY_B, XBOX_START, PS4_START, KEY_START, JOYLEFT, DPAD_LEFTRIGHT, DPAD_LEFTRIGHTUPDOWN, KEY_UPDOWNLEFTRIGHT,KEY_LEFTRIGHT;
	public Sprite L_JOY_CLICK, KEY_L_JOY_CLICK, R_JOY_CLICK, KEY_R_JOY_CLICK, XBOX_X, PS4_X, KEY_X, XBOX_Y, PS4_Y, KEY_Y, XBOX_BACK, PS4_BACK, KEY_BACK;
	public Sprite RB, LB, KEY_RB, KEY_LB, RT, LT, KEY_RT, KEY_LT;

	private ControllerManager cm;

	private PlayerControl[] players;

    void Awake()
    {
        if (instance == null)
        {
			DontDestroyOnLoad(this);
            instance = this;
            cm = new ControllerManager();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

		players = GameObject.FindObjectsOfType<PlayerControl>();
    }

	void Update() {
		if(cm.NumPlayers < 4) {
			if(cm.AddPlayer(ControllerInputWrapper.Buttons.Start)) {
				for(int i = 0; i < players.Length; i++) {
					if(players[i].player == (PlayerID)cm.NumPlayers) {
                        AddPlayer((PlayerID)cm.NumPlayers,players[i]);
					}
				}
			}
		}
	}

    /// <summary>
    /// Registers a player in the game.
    /// </summary>
    /// <param name="playerID">The ID of the player to register.</param>
    /// <param name="control">The control component of the player to register.</param>
    public void AddPlayer(PlayerID playerID, BaseControl control) {
        LevelManager.instance.playerDict.Add(playerID,control);
        LevelManager.instance.playerUI[(int)control.player - 1].SetupUI();
    }
}
