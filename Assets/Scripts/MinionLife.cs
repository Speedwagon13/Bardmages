using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MinionLife : PlayerLife {

	/// <summary>
	/// Sets the player health to half of parent and finds the appropriate UI elements
	/// </summary>
	protected override void Start() {
		base.Start ();
		health = health / 2;
	}
}