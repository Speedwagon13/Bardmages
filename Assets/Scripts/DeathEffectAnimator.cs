using UnityEngine;
using System.Collections;

/// <summary>
/// Animates the visual death effect.
/// </summary>
public class DeathEffectAnimator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.LookAt(Camera.main.transform, Camera.main.transform.up);
		StartCoroutine(AnimDeathEffect());
	}
	
    /// <summary>
    /// Animates the death effect growing and fading out.
    /// </summary>
	private IEnumerator AnimDeathEffect() {
		float timer = 0f;

		transform.GetChild(1).localPosition = new Vector3(10f,10f,0f);
		transform.GetChild(2).localPosition = new Vector3(-10f,10f,0f);
		transform.GetChild(3).localPosition = new Vector3(10f,-10f,0f);
		transform.GetChild(4).localPosition = new Vector3(-10f,-10f,0f);

		while (timer < 1f) {
			timer += Time.deltaTime;

			transform.GetChild(1).localPosition =
				Vector3.Lerp(transform.GetChild(1).localPosition, new Vector3(0.1f,0.1f,0f), timer);
			transform.GetChild(2).localPosition =
				Vector3.Lerp(transform.GetChild(2).localPosition, new Vector3(-0.1f,0.1f,0f), timer);
			transform.GetChild(3).localPosition =
				Vector3.Lerp(transform.GetChild(3).localPosition, new Vector3(0.1f,-0.1f,0f), timer);
			transform.GetChild(4).localPosition =
				Vector3.Lerp(transform.GetChild(4).localPosition, new Vector3(-0.1f,-0.1f,0f), timer);

			transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1,1,1,timer);
			transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(1,1,1,timer);
			transform.GetChild(3).GetComponent<SpriteRenderer>().color = new Color(1,1,1,timer);
			transform.GetChild(4).GetComponent<SpriteRenderer>().color = new Color(1,1,1,timer);
		
			yield return new WaitForEndOfFrame();
		}

		timer = 0f;

		while(timer < 1f) {
			timer += Time.deltaTime/5f;

			transform.localScale = Vector3.one*(1+timer);

            SpriteRenderer topRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            Color topColor = topRenderer.color;
            topColor.a = 1 - timer;
            topRenderer.color = topColor;
			transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1,1,1,1-(timer*2f));
			transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(1,1,1,1-(timer*2f));
			transform.GetChild(3).GetComponent<SpriteRenderer>().color = new Color(1,1,1,1-(timer*2f));
			transform.GetChild(4).GetComponent<SpriteRenderer>().color = new Color(1,1,1,1-(timer*2f));

			yield return new WaitForEndOfFrame();
		}

		Destroy(this.gameObject);
	}
}
