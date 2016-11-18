using UnityEngine;
using System.Collections;

public class AddCPUButton : MainMenuButton {

	private bool opened = false;
	private Coroutine flipAndChange;
	private Color initialColor;

    protected override void HandlePressed ()
    {
        base.HandlePressed ();
		if(flipAndChange != null) StopCoroutine(flipAndChange);
		flipAndChange = StartCoroutine(FlipAndChange());
    }

    private IEnumerator FlipAndChange() {
        float timer = 0f;
		if(initialColor == null && !opened) initialColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
		opened = !opened;
        while (timer < 1f) {
            timer += Time.deltaTime*2f;
			if(opened) {
				transform.localEulerAngles = new Vector3(0f,0f,Mathf.Lerp(transform.localEulerAngles.z, 180f, timer));
				transform.GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(initialColor, Color.red, timer);
			} else {
				transform.localEulerAngles = new Vector3(0f,0f,Mathf.Lerp(transform.localEulerAngles.z, 0f, timer));
				transform.GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(Color.red, initialColor, timer);
			}
            yield return new WaitForEndOfFrame();
        }
		yield return null;
    }
}