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

    // Use this for initialization
    void Start () {
        charControl = GetComponent<CharacterController>();
        move = Vector2.zero;
    }

    // Update is called once per frame
    void Update () {

        Vector2 rawInput = GetDirectionInput();
        move = Vector2.MoveTowards(move, rawInput, Time.deltaTime);

        if(move.magnitude > 1) move.Normalize();

        if(charControl) charControl.Move(new Vector3(move.x,-1f,move.y) * Time.deltaTime * speed);


        if(rawInput != Vector2.zero) {
            float targetRotation = Vector2.Angle(Vector2.left, new Vector2(-rawInput.y,rawInput.x));
            float dir = Mathf.Sign(rawInput.x);
            float targetYaw = targetRotation*dir;
            float yaw = GetGradualTurn() ? Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetYaw, Time.deltaTime*500f) : targetYaw;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yaw, transform.eulerAngles.z);
        }
    }

    public void Knockback(Vector2 direction) {
        move += direction;
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
}