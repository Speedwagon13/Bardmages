using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PhysicalButton : MonoBehaviour {

	public enum ButtonState {Normal, Hover, Pressed};
	private ButtonState state;

	public UnityEvent onPress;

	// Use this for initialization
	protected void Start () {
		state = ButtonState.Normal;
	}
	
	// Update is called once per frame
	protected void Update () {
		switch(state) {

		case ButtonState.Normal:
			HandleNormal();
			break;
		case ButtonState.Hover:
			HandleHover();
			break;
		case ButtonState.Pressed:
			HandlePressed();
			break;

		}
	}

	protected virtual void HandleNormal() {}
	protected virtual void HandleHover() {}
	protected virtual void HandlePressed() {}

	void OnMouseOver() {
		state = ButtonState.Hover;
	}

	void OnMouseExit() {
		state = ButtonState.Normal;
	}

	void OnMouseDown() {
		onPress.Invoke();
		HandlePressed();
		state = ButtonState.Pressed;
	}
}
