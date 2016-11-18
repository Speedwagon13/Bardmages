using UnityEngine;

/// <summary>
/// Base class for moving a bardmage.
/// </summary>
public abstract class BaseControl : MonoBehaviour {

    public PlayerID player;
    public float speed;

    private Vector2 move;
    private CharacterController charControl;

    private Vector2 knockback;

    /// <summary>
    /// The main player who owns this control.
    /// If this is a minion, returns the player who spawned the minion instead of None.
    /// </summary>
    public PlayerID playerOwner {
        get { 
            if (player == PlayerID.None) {
                return GetComponent<MinionTuneSpawn>().owner;
            } else {
                return player;
            }
        }
    }

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    protected virtual void Start () {
        charControl = GetComponent<CharacterController>();
        move = Vector2.zero;
    }

    // Update is called once per frame.
    void Update () {

        Vector2 rawInput = GetDirectionInput();
        move = Vector2.MoveTowards(move, rawInput, Time.deltaTime);

//        if(move.magnitude > 1) move.Normalize();

        if(charControl) charControl.Move(new Vector3(move.x,-1f,move.y) * Time.deltaTime * speed);


        if(rawInput != Vector2.zero) {
            float targetRotation = Vector2.Angle(Vector2.left, new Vector2(-rawInput.y,rawInput.x));
            float dir = Mathf.Sign(rawInput.x);
            float targetYaw = targetRotation*dir;
            float yaw = GetGradualTurn() ? Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetYaw, Time.deltaTime*500f) : targetYaw;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yaw, transform.eulerAngles.z);
        }
    }

    /// <summary>
    /// Adds knockback to the player's movement.
    /// </summary>
    /// <param name="direction">The direction of the knockback.</param>
    /// <param name="tune">The tune that caused the knockback.</param>
    /// <param name="owner">The player who caused the knockback.</param>
    public void Knockback(Vector2 direction, Tune tune = null, PlayerID owner = PlayerID.None) {
        if (owner == PlayerID.None) {
            owner = player;
        }
        move += direction;
        PlayerLife life = GetComponent<PlayerLife>();
        life.lastHitTune = tune;
        life.lastHitPlayer = owner;
    }

	public void ClearMomentum() {
		move = Vector2.zero;
	}

    /// <summary>
    /// Gets the directional input to move the bardmage with.
    /// </summary>
    /// <returns>The directional input to move the bardmage with.</returns>
    protected abstract Vector2 GetDirectionInput();

    /// <summary>
    /// Checks if the bardmage turns gradually.
    /// </summary>
    /// <returns>Whether the bardmage turns gradually.</returns>
    protected abstract bool GetGradualTurn();

    /// <summary>
    /// Gets the material that makes up the bardmage's robe.
    /// </summary>
    /// <returns>The material that makes up the bardmage's robe..</returns>
    public Material GetRobeMaterial() {
        return transform.FindChild("bardmage_export").FindChild("pCube2").GetComponent<Renderer>().material;
    }
}