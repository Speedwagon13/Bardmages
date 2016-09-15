using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour {

	/// <summary>
	/// This players' health.
	/// This is normalized (0-1);
	/// </summary>
	private float health;

	/// <summary>
	/// The ui elements to be animated when the player is damaged
	/// </summary>
	private Image greenHealthBar, freshRedHealthBar;

	/// <summary>
	/// Sets the player health to 1 and finds the appropriate UI elements
	/// </summary>
	void Start() {
		health = 1f;
		greenHealthBar = transform.FindChild("Canvas").FindChild("HealthBarRed").FindChild("HealthBarGreen").GetComponent<Image>();
		freshRedHealthBar = transform.FindChild("Canvas").FindChild("HealthBarRed").FindChild("HealthBarFreshRed").GetComponent<Image>();
	}

	/// <summary>
	/// Deals damage to this player.
	/// </summary>
	/// <param name="amount">The amount of damage done. All damage is normalized (1 = instakill)</param>
	public void DealDamage(float amount) {
		health -= amount;
		bool died = false;
		if(health <= 0) {
			transform.position = Vector3.up*10f;
			health = 1f;
			died = true;
		}

		greenHealthBar.fillAmount = health/2f + 0.5f;
		StartCoroutine(HealthBarCatchup());

		if(LevelManager.instance.playerUI[((int)GetComponent<BaseControl>().player) - 1] != null) {
			LevelManager.instance.playerUI[((int)GetComponent<BaseControl>().player) - 1].UpdateHealth(health, died);
		}
	}

	/// <summary>
	/// Raises the controller collider hit event.
	/// </summary>
	/// <param name="hit">Hit.</param>
	void OnControllerColliderHit(ControllerColliderHit hit) {
		if(hit.collider.gameObject.tag.Equals("Kill")) {
			DealDamage(1f);
		}
	}

	/// <summary>
	/// A coroutine that handles animating the health bar
	/// </summary>
	/// <returns>Coroutine.</returns>
	private IEnumerator HealthBarCatchup() {
		float timer = 0f;

		while (timer < 1f) {
			timer += Time.deltaTime;

			freshRedHealthBar.fillAmount = Mathf.Lerp(freshRedHealthBar.fillAmount, greenHealthBar.fillAmount, timer);

			yield return new WaitForEndOfFrame();
		}

		yield return null;
	}

}
