using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Bardmages.AI;

public class PlayerLife : MonoBehaviour {

	/// <summary>
	/// This players' health.
	/// This is normalized (0-1);
	/// </summary>
	protected float health;

	protected Vector3 positionOfDeath;

    public bool fellOffMap;
    public bool roundHandler;

    /// <summary>
    /// The ui elements to be animated when the player is damaged
    /// </summary>
    private Image greenHealthBar, freshRedHealthBar;

    /// <summary>
    /// (DEPRECATED)Handles offsetting the respawn, if the player is allowed to respawn
    /// </summary>
    private float respawnTimer;

    /// <summary>
    /// (DEPRECATED)How much time should the player remain dead?
    /// Use -1 for game modes like elimination where players don't respawn automatically
    /// </summary>
    public float respawnTime;

    /// <summary> The movement component of the player. </summary>
    private BaseControl control;

    /// <summary> The controller for this player's UI health bar. </summary>
    private PlayerUIController uiController;

    /// <summary> The tune that most recently hit the player. </summary>
    [HideInInspector]
    public Tune lastHitTune;
    /// <summary> The opponent who most recently hit the player. </summary>
    [HideInInspector]
    public PlayerID lastHitPlayer;

	/// <summary>
	/// Sets the player health to 1 and finds the appropriate UI elements
	/// </summary>
	protected virtual void Start()
    {
        roundHandler = Assets.Scripts.Data.RoundHandler.Instance != null;
        control = GetComponent<BaseControl>();
        uiController = LevelManager.instance.GetPlayerUI(control.player);
		health = 1f;
		greenHealthBar = transform.FindChild("Canvas").FindChild("HealthBarRed").FindChild("HealthBarGreen").GetComponent<Image>();
		freshRedHealthBar = transform.FindChild("Canvas").FindChild("HealthBarRed").FindChild("HealthBarFreshRed").GetComponent<Image>();
	}

    /// <summary>
    /// Deals damage to this player.
    /// </summary>
    /// <param name="amount">The amount of damage done. All damage is normalized (1 = instakill)</param>
    /// <param name="sourceTune">The tune that caused the damage.</param>
    /// <param name="aggressor">The player who caused the damage.</param>
    public void DealDamage(float amount, Tune sourceTune = null, PlayerID aggressor = PlayerID.None) {
        BaseControl control = GetComponent<BaseControl>();

        if (sourceTune != null && aggressor != control.player && amount > 0) {
            BubbleShield shield = GetComponentInChildren<BubbleShield>();
            if (shield != null) {
                shield.BlockAttack(amount);
                return;
            }
        }

		health -= amount;
		bool died = false;
		if(health <= 0) {
			control.ClearMomentum();
            if (control.player != PlayerID.None) {
                EffectManager.instance.SpawnDeathEffect (transform.position, control.GetRobeMaterial().color);
			}
			GetComponent<BaseControl>().enabled = false;
			positionOfDeath = transform.position;
			transform.position = Vector3.up*100f;
			died = true;
            if(roundHandler)
                Assets.Scripts.Data.RoundHandler.Instance.AddDeath(control.player);
            else
                respawnTimer = respawnTime;
        }
        if (health > 1) {
            health = 1f;
        }

        updateHealthBar();

        if(uiController != null) {
            uiController.UpdateHealth(health, died);
        }

        if (aggressor == PlayerID.None) {
            aggressor = control.player;
        } else {
            float weight = amount;
            if (aggressor == control.player) {
                weight = -weight;
            }
            LevelManager.instance.GetBardFromID(aggressor).CreditHit(sourceTune, weight, true);
        }

        if (amount > 0) {
            lastHitTune = sourceTune;
            lastHitPlayer = aggressor;

            if (aggressor != control.player) {
                AdaptiveAI adapt = GetComponent<AdaptiveAI>();
                if (adapt != null) {
                    adapt.TakeDamage(amount);
                }
            }
        }
	}

    void Update()
    {
        if (!roundHandler && (health <= 0f && respawnTimer > 0f))
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0f)
            {
                transform.position = Vector3.up * 10f;
                Respawn();
                GetComponent<BaseControl>().enabled = true;
            }
        }
    }

    /// <summary> Re-initialize the player health and re-enable control. </summary>
    public void Respawn()
    {
        health = 1f;
        greenHealthBar.fillAmount = 1f;
        PlayerUIController uiController = LevelManager.instance.GetPlayerUI(GetComponent<BaseControl>().player);
        if (uiController != null)
        {
            uiController.UpdateHealth(health, false);
        }
    }

    /// <summary> Updates the health bar around the player with the player's current health. </summary>
    private void updateHealthBar() {
        greenHealthBar.fillAmount = health/2f + 0.5f;
        StartCoroutine(HealthBarCatchup());
    }

	/// <summary>
	/// Raises the controller collider hit event.
	/// </summary>
	/// <param name="hit">Hit.</param>
	void OnControllerColliderHit(ControllerColliderHit hit) {
		if(hit.collider.gameObject.tag.Equals("Kill")) {
			fellOffMap = true;
            if (lastHitTune != null) {
                LevelManager.instance.GetBardFromID(lastHitPlayer).CreditHit(lastHitTune, 1f, true);
            }
			DealDamage(1f);

		}
	}

	public bool Alive {
		get {
			return health > 0;
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

	public float Health {
		get {
			return health;
		}
	}

	public Vector3 PositionOfDeath {
		get {
			return positionOfDeath;
		}
	}

	public bool FellOffMap {
		get {
			return fellOffMap;
		}
		set {
			fellOffMap = value;
		}
	}
}
