using UnityEngine;
using System.Collections;
using Bardmages.AI;

public class MainMenuManager : MonoBehaviour {

	/// <summary>
	/// The blocks that animate when you add a new player
	/// </summary>
	public GameObject[] playerBlocks;

	public Sprite[] levelImages;
	private int curLevel;

	/// <summary>
	/// A list of all the tunes in the game.
	/// </summary>
	private Tune[] tunes;

	/// <summary>
	/// The array of the boxes that hold the tunes for each player
	/// </summary>
	private GameObject[,] playerTunes;

	/// <summary>
	/// The various visuals to communicate the tunes to the players
	/// </summary>
	private GameObject[] playerTuneDescriptions, playerTuneName, upIcons, downIcons, acceptIcons, rejectIcons;
	private GameObject[] playerReadyText, pressStart;

	private GameObject levelSelectFrame, nameFrame;

	/// <summary>
	/// The selected tunes for each player.
	/// </summary>
	private int[,] selectedTune;

	/// <summary>
	/// Which tune each player is setting next
	/// </summary>
	private int[] nextTune;

	/// <summary>
	/// Silly state variable
	/// </summary>
	private bool inPlayerSelect, inLevelSelect;

	/// <summary>
	/// Ensures repeat input doesn't occurr accidentally
	/// </summary>
	private float[] inputDelay;

	private int numAI;

	#region State_Changing
	public void GoToMainMenu() {
		GetComponent<Animator>().SetInteger("State",0);
	}

	public void GoToPlayMenu() {
		GetComponent<Animator>().SetInteger("State",1);
	}

	public void GoToPlayerSelect() {
		inPlayerSelect = true;
		GetComponent<Animator>().SetInteger("State",2);
	}

	public void GoToGame() {
		Assets.Scripts.Data.Data.Instance.NumOfPlayers = ControllerManager.instance.NumPlayers;
		Assets.Scripts.Data.Data.Instance.loadScene(levelImages[curLevel].name);
	}

	public void ToggleAI(int num) {
		bool isAi = Assets.Scripts.Data.Data.Instance.isAIPlayer[num];
		Assets.Scripts.Data.Data.Instance.isAIPlayer[num] = !isAi;
		if(!isAi) {
			int[] aiTunes = new int[3];
			for(int i = 0; i < 3; i++) {
				aiTunes[i] = Random.Range(0,tunes.Length-2);
				for(int j = 0; j < i; j++) {
					while(aiTunes[i] == aiTunes[j]) {
						aiTunes[i]++;
						aiTunes[i] %= (tunes.Length-2);
					}
				}
			}
			Assets.Scripts.Data.Data.Instance.AddTuneToPlayer((PlayerID)(num+1), tunes[aiTunes[0]], 0);
			Assets.Scripts.Data.Data.Instance.AddTuneToPlayer((PlayerID)(num+1), tunes[aiTunes[1]], 1);
			Assets.Scripts.Data.Data.Instance.AddTuneToPlayer((PlayerID)(num+1), tunes[aiTunes[2]], 2);
			numAI++;
			ControllerManager.instance.AddAI(true);
			StartCoroutine(PlayerReady(num));
		} else {
			numAI--;
			ControllerManager.instance.AllowAIRemoval(true);
			StartCoroutine(AddPlayerAnim());
		}
	}

	public void SwitchLevel(int delta) {
		curLevel += delta;
		if(curLevel < 0) curLevel = levelImages.Length;
		curLevel %= levelImages.Length;
		StartCoroutine(ChangeSelectedLevel(delta > 0 ? true : false));
	}

	public void GoToLevelSelect() {
		inPlayerSelect = false;
		GetComponent<Animator>().SetInteger("State",3);
	}
	#endregion
		

	#region Unity_Code
	void Start() {
		selectedTune = new int[4,3];
		nextTune = new int[4];
		for(int i = 0; i < 4; i++) {
			nextTune[i] = 0;
			for(int j = 0; j < 3; j++) {
				selectedTune[i,j] = j;
			}
		}

		inputDelay = new float[4];

		playerTunes = new GameObject[4,3];
		playerTuneDescriptions = new GameObject[4];
		playerReadyText = new GameObject[4];
		pressStart = new GameObject[4];
		upIcons = new GameObject[4];
		downIcons = new GameObject[4];
		acceptIcons = new GameObject[4];
		rejectIcons = new GameObject[4];

		for(int i = 0; i < 4; i++) {
			for(int j = 0; j < 3; j++) {
				playerTunes[i,j] = playerBlocks[i].transform.FindChild("Tune"+(j+1)).gameObject;
			}
		}

		for(int i = 0; i < 4; i++) {
			playerTuneDescriptions[i] = playerBlocks[i].transform.FindChild("TuneDescription").gameObject;
			playerReadyText[i] = playerBlocks[i].transform.FindChild("Player" + (i+1) + "Ready").gameObject;
			pressStart[i] = playerBlocks[i].transform.FindChild("Player1Join_Text").gameObject;
			upIcons[i] = playerBlocks[i].transform.FindChild("UpIcon").gameObject;
			downIcons[i] = playerBlocks[i].transform.FindChild("DownIcon").gameObject;
			acceptIcons[i] = playerBlocks[i].transform.FindChild("ControlDescription").FindChild("Button_A").gameObject;
			rejectIcons[i] = playerBlocks[i].transform.FindChild("ControlDescription").FindChild("Button_B").gameObject;
			if (i > 0) playerBlocks[i].transform.FindChild("CPU").gameObject.SetActive(false);
		}

		tunes = Assets.Scripts.Data.Data.Instance.GetAllTunes();

		levelSelectFrame = transform.FindChild("LevelSelect/LevelSelectContainer/Frame").gameObject;
		nameFrame = transform.FindChild("LevelSelect/LevelSelectContainer/MapBoxContainer/MapNameBox").gameObject;
	}

	void Update() {
		if(inPlayerSelect) {
			if(ControllerManager.instance.AddPlayer(ControllerInputWrapper.Buttons.Start)) {
				StopAllCoroutines();
				StartCoroutine(AddPlayerAnim());
			}

			for(int i = 0; i < ControllerManager.instance.NumPlayers; i++) {
				if(inputDelay[i] > 0f) inputDelay[i] -= Time.deltaTime;
				if(nextTune[i] < 3) {
					if(ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY, (PlayerID)(i+1)) > 0
						&& inputDelay[i] <= 0f) {
						inputDelay[i] = 0.25f;
						selectedTune[i,nextTune[i]] = (selectedTune[i,nextTune[i]]+1) % tunes.Length;
						StartCoroutine(UpdateSelectedTune(selectedTune[i,nextTune[i]], i, true));
					}
					if(ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY, (PlayerID)(i+1)) < 0
						&& inputDelay[i] <= 0f) {
						inputDelay[i] = 0.25f;
						if(selectedTune[i,nextTune[i]] <= 0) selectedTune[i,nextTune[i]] = tunes.Length-1;
						else selectedTune[i,nextTune[i]]--;
						StartCoroutine(UpdateSelectedTune(selectedTune[i,nextTune[i]], i, false));
					}
					if(ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.A, (PlayerID)(i+1))) {
						nextTune[i]++;
						if(nextTune[i] == 3) {
							StartCoroutine(PlayerReady(i));
							playerBlocks[i].transform.FindChild("CPU").gameObject.SetActive(false);
							Assets.Scripts.Data.Data.Instance.AddTuneToPlayer((PlayerID)(i+1), tunes[selectedTune[i,0]], 0);
							Assets.Scripts.Data.Data.Instance.AddTuneToPlayer((PlayerID)(i+1), tunes[selectedTune[i,1]], 1);
							Assets.Scripts.Data.Data.Instance.AddTuneToPlayer((PlayerID)(i+1), tunes[selectedTune[i,2]], 2);
						} else {
							StartCoroutine(UpdateNextTune(i));
						}
					}
				}
				if(ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, (PlayerID)(i+1))) {
					if(nextTune[i] > 0) {
						if(nextTune[i] == 3) {
							StartCoroutine(PlayerUnready(i));
						}
						nextTune[i]--;
						StartCoroutine(UpdateNextTune(i));
					} else {
						playerBlocks[i].transform.FindChild("CPU").gameObject.SetActive(false);
						if(ControllerManager.instance.AllowPlayerRemoval(ControllerInputWrapper.Buttons.B) != 0) {
							StopAllCoroutines();
							StartCoroutine(AddPlayerAnim());
						}
					}
				}
			}
		}
		if(inLevelSelect) {
			
		}
	}
	#endregion


	#region Animation_Coroutines
	private IEnumerator PlayerReady(int player) {
		float timer = 0f;

		while(timer < 1f) {
			timer += Time.deltaTime;

			playerBlocks[player].transform.localRotation = Quaternion.Lerp(playerBlocks[player].transform.localRotation,
				Quaternion.Euler(new Vector3(0f,0f,0f)),
				timer);

			yield return new WaitForEndOfFrame();
		}

		playerTuneDescriptions[player].GetComponent<Renderer>().enabled = false;
		playerReadyText[player].GetComponent<Renderer>().enabled = true;
		pressStart[player].GetComponent<Renderer>().enabled = false;
		if(player < 3) playerBlocks[player+1].transform.FindChild("CPU").gameObject.SetActive(true);

		yield return null;
	}

	private IEnumerator PlayerUnready(int player) {
		float timer = 0f;

		while(timer < 1f) {
			timer += Time.deltaTime;

			playerBlocks[player].transform.localRotation = Quaternion.Lerp(playerBlocks[player].transform.localRotation,
				Quaternion.Euler(new Vector3(-180f,0f,0f)),
				timer);
			playerTuneDescriptions[player].GetComponent<Renderer>().enabled = true;
			yield return new WaitForEndOfFrame();
		}

		yield return null;
	}

	private IEnumerator UpdateNextTune(int player) {
		float timer = 0f;

		while(timer < 1f) {
			timer += Time.deltaTime*8f;
			playerTunes[player,nextTune[player]].transform.localPosition = Vector3.Lerp(playerTunes[player,nextTune[player]].transform.localPosition,
				new Vector3(0, -3f, -4.3f + 3*nextTune[player]), timer);
			upIcons[player].transform.localPosition = 
				Vector3.Lerp(upIcons[player].transform.localPosition,
					new Vector3(0f, -4f, playerTunes[player,nextTune[player]].transform.localPosition.z - 1.2f), timer);
			downIcons[player].transform.localPosition = 
				Vector3.Lerp(downIcons[player].transform.localPosition,
					new Vector3(0f, -4f, playerTunes[player,nextTune[player]].transform.localPosition.z + 0.7f), timer);
			for(int i = 0; i < 3; i++) {
				if(i != nextTune[player]) {
					playerTunes[player,i].transform.localPosition = Vector3.Lerp(playerTunes[player,i].transform.localPosition,
						new Vector3(0, -2.1f, -4.3f + 3*i), timer);
				}
			}
			yield return new WaitForEndOfFrame();
		}

		yield return null;
	}

	private IEnumerator UpdateSelectedTune(int tuneIndex, int player, bool increase) {
		float timer = 0f;

		while (timer < 1f) {
			timer += Time.deltaTime*8f;

			playerTunes[player, nextTune[player]].transform.localRotation =
				Quaternion.Lerp(playerTunes[player, nextTune[player]].transform.localRotation,
					Quaternion.Euler(increase ? -180f : 180f,0f,0f),
					timer);
			playerTuneDescriptions[player].transform.localRotation =
				Quaternion.Lerp(playerTunes[player, nextTune[player]].transform.localRotation,
					Quaternion.Euler(increase ? 270f : -270f,0f,0f),
					timer);
			yield return new WaitForEndOfFrame();
		}

		timer = 0f;
		playerTunes[player, nextTune[player]].transform.GetChild(0).GetComponent<TextMesh>().text = tunes[tuneIndex].tuneName;
		playerTuneDescriptions[player].GetComponent<TextMesh>().text = tunes[tuneIndex].tuneDescription.Replace("\\n","\n");

		while (timer < 1f) {
			timer += Time.deltaTime*8f;

			playerTunes[player, nextTune[player]].transform.localRotation =
				Quaternion.Lerp(playerTunes[player, nextTune[player]].transform.localRotation,
					Quaternion.Euler(0f,0f,0f),
					timer);
			playerTuneDescriptions[player].transform.localRotation =
				Quaternion.Lerp(playerTunes[player, nextTune[player]].transform.localRotation,
					Quaternion.Euler(-90f,0f,0f),
					timer);

			yield return new WaitForEndOfFrame();
		}
		yield return null;
	}

	private IEnumerator AddPlayerAnim() {
		float timer = 0f;

		while(timer < 1f) {
			timer += Time.deltaTime;
			int i = 0;
			for(; i < ControllerManager.instance.NumPlayers; i++) {
				if(nextTune[i] != 3 && !Assets.Scripts.Data.Data.Instance.isAIPlayer[i]) {
					playerBlocks[i].transform.localRotation = Quaternion.Lerp(playerBlocks[i].transform.localRotation,
						Quaternion.Euler(new Vector3(-180f,0f,0f)),
						timer);
					playerTuneDescriptions[i].GetComponent<Renderer>().enabled = true;
				}
			}
			if(i < 4) playerBlocks[i].transform.FindChild("CPU").gameObject.SetActive(true);
			for(; i < 4; i++) {
				playerBlocks[i].transform.localRotation = Quaternion.Lerp(playerBlocks[i].transform.localRotation,
					Quaternion.Euler(new Vector3(0f,0f,0f)),
					timer);
				playerTuneDescriptions[i].GetComponent<Renderer>().enabled = false;
				playerReadyText[i].GetComponent<Renderer>().enabled = false;
				pressStart[i].GetComponent<Renderer>().enabled = true;
				if(i+1 < 4) playerBlocks[i+1].transform.FindChild("CPU").gameObject.SetActive(false);
			}
			yield return new WaitForEndOfFrame();
		}

		yield return null;
	}

	private IEnumerator ChangeSelectedLevel(bool increase) {
		float timer = 0f;

		while (timer < 1f) {
			timer += Time.deltaTime * 4f;
			levelSelectFrame.transform.localEulerAngles = new Vector3(0f,0f,Mathf.Lerp(0f,increase ? 180f : -180f,timer));
			levelSelectFrame.transform.localPosition = new Vector3(0f,Mathf.Lerp(0f,-2f,0f),0f);
			yield return new WaitForEndOfFrame();
		}
		timer = 0f;
		levelSelectFrame.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = levelImages[curLevel];
		nameFrame.transform.GetChild(0).GetComponent<TextMesh>().text = levelImages[curLevel].name;
		while (timer < 1f) {
			timer += Time.deltaTime * 4f;
			levelSelectFrame.transform.localEulerAngles = new Vector3(0f,0f,Mathf.Lerp(increase ? 180f : -180f,increase ? 360f : -360f,timer));
			levelSelectFrame.transform.localPosition = new Vector3(0f,Mathf.Lerp(0f,0f,0f),0f);
			yield return new WaitForEndOfFrame();
		}

		yield return null;
	}
	#endregion

}
