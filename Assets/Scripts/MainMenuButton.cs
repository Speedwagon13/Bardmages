using UnityEngine;
using System.Collections;

public class MainMenuButton : PhysicalButton {

    public float raisedHeight = 1f;

	private Vector3 initialPos;

    new void Start() {
        base.Start();
		initialPos = transform.localPosition;
    }

    protected override void HandleHover ()
    {
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, initialPos + Vector3.up*raisedHeight, Time.deltaTime*15f);
        base.HandleHover ();
    }

    protected override void HandleNormal ()
    {
		float temp = LevelManager.instance.BeatValue((transform.GetSiblingIndex()%2)*5512f);
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, initialPos + Vector3.up*temp, Time.deltaTime*15f);
        base.HandleNormal ();
    }

    protected override void HandlePressed ()
    {
        base.HandlePressed ();
        StopCoroutine(ButtonPressed());
        StartCoroutine(ButtonPressed());
    }

    private IEnumerator ButtonPressed() {
        float timer = 0f;
        Vector3 startingScale = transform.localScale;
        transform.localScale *= 1.5f;

        while (timer < 1f) {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, startingScale, timer);
            yield return new WaitForEndOfFrame();
        }
    }
}