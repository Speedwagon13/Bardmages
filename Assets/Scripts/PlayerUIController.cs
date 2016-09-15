using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour {

	public PlayerID player;

	public bool rightSideUI;

	private Tune[] tunes;
	private Image[] tune1Keys, tune2Keys, tune3Keys;
	private Image healthBar;
	private Text deathCounter;
	private RectTransform[] redFill;
	private float redFillInitialLength;

	private RectTransform circle;

	private int deaths = 0;

    /// <summary>
    /// Initializes UI fields to track the player.
    /// </summary>
	public void SetupUI () {
		tunes = LevelManager.instance.playerDict[player].GetComponent<BaseBard>().tunes;
		tune1Keys = new Image[tunes[0].tune.Length];
		tune2Keys = new Image[tunes[1].tune.Length];
		tune3Keys = new Image[tunes[2].tune.Length];

		circle = transform.FindChild("ColorCircle").GetComponent<RectTransform>();

		redFill = new RectTransform[3];
		redFill[0] = transform.FindChild("Tune1").FindChild("RedFill").GetComponent<RectTransform>();
		redFill[1] = transform.FindChild("Tune2").FindChild("RedFill").GetComponent<RectTransform>();
		redFill[2] = transform.FindChild("Tune3").FindChild("RedFill").GetComponent<RectTransform>();
		redFillInitialLength = redFill[0].sizeDelta.x;
		redFill[0].sizeDelta = new Vector2(0f,redFill[0].sizeDelta.y);
		redFill[1].sizeDelta = new Vector2(0f,redFill[1].sizeDelta.y);
		redFill[2].sizeDelta = new Vector2(0f,redFill[2].sizeDelta.y);


		tune1Keys[0] = transform.FindChild("Tune1").FindChild("ButtonHolder").GetChild(0).GetComponent<Image>();
		tune1Keys[0].GetComponent<AutoKeyUI>().button = tunes[0].tune[0];
		for(int i = 1; i < tune1Keys.Length; i++) {
			GameObject temp = (GameObject)GameObject.Instantiate(tune1Keys[0].gameObject, tune1Keys[0].transform.parent, true);
			tune1Keys[i] = temp.GetComponent<Image>();
			tune1Keys[i].GetComponent<AutoKeyUI>().button = tunes[0].tune[i];
			tune1Keys[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(rightSideUI ? 30 + 40*i : -(30 + 40*i),0f);
			tune1Keys[i].color = new Color(1,1,1,1-((float)i/(float)tune1Keys.Length));
		}
		tune2Keys[0] = transform.FindChild("Tune2").FindChild("ButtonHolder").GetChild(0).GetComponent<Image>();
		tune2Keys[0].GetComponent<AutoKeyUI>().button = tunes[1].NextButton();
		for(int i = 1; i < tune2Keys.Length; i++) {
			GameObject temp = (GameObject)GameObject.Instantiate(tune2Keys[0].gameObject, tune2Keys[0].transform.parent, true);
			tune2Keys[i] = temp.GetComponent<Image>();
			tune2Keys[i].GetComponent<AutoKeyUI>().button = tunes[1].tune[i];
			tune2Keys[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(rightSideUI ? 30 + 40*i : -(30 + 40*i),0f);
			tune2Keys[i].color = new Color(1,1,1,1-((float)i/(float)tune2Keys.Length));
		}
		tune3Keys[0] = transform.FindChild("Tune3").FindChild("ButtonHolder").GetChild(0).GetComponent<Image>();
		tune3Keys[0].GetComponent<AutoKeyUI>().button = tunes[2].NextButton();
		for(int i = 1; i < tune3Keys.Length; i++) {
			GameObject temp = (GameObject)GameObject.Instantiate(tune3Keys[0].gameObject, tune3Keys[0].transform.parent, true);
			tune3Keys[i] = temp.GetComponent<Image>();
			tune3Keys[i].GetComponent<AutoKeyUI>().button = tunes[2].tune[i];
			tune3Keys[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(rightSideUI ? 30 + 40*i : -(30 + 40*i),0f);
			tune3Keys[i].color = new Color(1,1,1,1-((float)i/(float)tune3Keys.Length));
		}

		healthBar = transform.FindChild("ColorCircle").FindChild("HealthBar").GetComponent<Image>();

		deathCounter = transform.FindChild("Deaths").GetComponent<Text>();
	}

	void Update() {
		if(circle)
			circle.localScale = Vector3.one*(1+LevelManager.instance.BeatValue(0));
	}

	public void TuneProgressed(Tune t) {
		for(int i = 0; i < tunes.Length; i++) {
			if(tunes[i].Equals(t)) {
				StopCoroutine(ResetTuneTimer(i));
				StartCoroutine(ResetTuneTimer(i));
				StopCoroutine(ProgressTuneAnim(i+1,t.tuneProgress));
				StartCoroutine(ProgressTuneAnim(i+1,t.tuneProgress));
			}
		}
	}

	public void TuneReset() {
		StopAllCoroutines();
		for(int i = 0; i < tune1Keys.Length; i++) {
			tune1Keys[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(rightSideUI ? 30 + 40*i : -(30 + 40*i),0f);
			tune1Keys[i].color = new Color(1,1,1,1-((float)i/(float)tune1Keys.Length));
			tune1Keys[i].GetComponent<RectTransform>().localScale = Vector2.one;
		}
		for(int i = 0; i < tune2Keys.Length; i++) {
			tune2Keys[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(rightSideUI ? 30 + 40*i : -(30 + 40*i),0f);
			tune2Keys[i].color = new Color(1,1,1,1-((float)i/(float)tune2Keys.Length));
			tune2Keys[i].GetComponent<RectTransform>().localScale = Vector2.one;
		}
		for(int i = 0; i < tune3Keys.Length; i++) {
			tune3Keys[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(rightSideUI ? 30 + 40*i : -(30 + 40*i),0f);
			tune3Keys[i].color = new Color(1,1,1,1-((float)i/(float)tune3Keys.Length));
			tune3Keys[i].GetComponent<RectTransform>().localScale = Vector2.one;
		}

		for(int i = 0; i < 3; i++) {
			redFill[i].sizeDelta = new Vector2(0f,redFill[i].sizeDelta.y);
		}
	}

	private IEnumerator ResetTuneTimer(int tune) {
		float timer = 0f;

		while (timer < LevelManager.instance.Tempo*2f) {
			timer += Time.deltaTime;
			redFill[tune].sizeDelta = new Vector2(Mathf.Lerp(0f, redFillInitialLength, timer/LevelManager.instance.Tempo*2f),redFill[tune].sizeDelta.y);
			yield return new WaitForEndOfFrame();
		}

		yield return null;
	}

	private IEnumerator ProgressTuneAnim(int tune, int index) {
		Image[] tuneKeys;

		if(tune == 1) tuneKeys = tune1Keys;
		else if(tune == 2) tuneKeys = tune2Keys;
		else tuneKeys = tune3Keys;

		redFill[tune-1].sizeDelta = new Vector2(0f,redFill[0].sizeDelta.y);

		float animatePressedButton = 0f;

		while(animatePressedButton < 1f) {
			animatePressedButton += Time.deltaTime;
			tuneKeys[index].transform.localScale = Vector3.one*(animatePressedButton+1);
			tuneKeys[index].color = new Color(1,1,1,1-animatePressedButton);
			for(int i = index + 1; i < tuneKeys.Length; i++) {
				tuneKeys[i].GetComponent<RectTransform>().anchoredPosition =
					Vector2.Lerp(tuneKeys[i].GetComponent<RectTransform>().anchoredPosition,
						new Vector2(rightSideUI ? 30 + 40*(i-(index+1)) : -(30 + 40*(i-(index+1))),0f), animatePressedButton);
				tuneKeys[i].color = new Color(1,1,1,1-((float)(i-(index+1))/(float)tuneKeys.Length));
			}
			yield return new WaitForEndOfFrame();
		}
			
		yield return null;
	}

	public void UpdateHealth(float amount, bool died) {
		if(healthBar) {
			healthBar.fillAmount = amount;
			deaths++;
			if(died) deathCounter.text = deaths.ToString();
		}
	}


}
