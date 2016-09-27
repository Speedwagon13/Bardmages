﻿using UnityEngine;

/// <summary>
/// Allows a player to move a bardmage.
/// </summary>
public class AIControl : BaseControl {

    /// <summary> The direction that the bardmage is moving in. </summary>
    [HideInInspector]
    public Vector2 currentDirection = Vector2.zero;

    /// <summary> The navigator for the level terrain. </summary>
    private NavMeshAgent navMeshAgent;
    /// <summary> The path currently being taken by the bardmage. </summary>
    private NavMeshPath currentPath;
    /// <summary> The index of the current path node in the path being navigated to. </summary>
    private int currentNodeIndex = -1;
    /// <summary> The path node that the bardmage is heading towards. </summary>
    private Vector3 currentNode {
        get { return currentPath.corners[currentNodeIndex]; }
    }
    /// <summary> Whether the bardmage is currently moving to a position. </summary>
    public bool isMoving {
        get { return currentNodeIndex > -1; }
    }

    /// <summary> Whether the bardmage is currently turning to face a position. </summary>
    private bool _isTurning;
    public bool isTurning {
        get { return _isTurning; }
    }

    /// <summary> Whether the bardmage is currently executing an action. </summary>
    public bool isBusy {
        get { return isMoving || _isTurning; }
    }

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    protected override void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
        currentPath = new NavMeshPath();
        base.Start();
    }

    /// <summary>
    /// Gets the directional input to move the bardmage with.
    /// </summary>
    /// <returns>The directional input to move the bardmage with.</returns>
    protected override Vector2 GetDirectionInput() {
        return currentDirection;
    }

    /// <summary>
    /// Checks if the bardmage turns gradually.
    /// </summary>
    /// <returns>Whether the bardmage turns gradually.</returns>
    protected override bool GetGradualTurn() {
        return true;
    }

    /// <summary>
    /// Updates control aspects of the AI.
    /// </summary>
    public void UpdateControl() {
        if (_isTurning && Vector3.Angle(GetDirection2D(transform.forward), currentDirection) < 0.1f) {
            _isTurning = false;
            currentDirection = Vector3.zero;
        } else if (isMoving) {
            currentDirection = GetFacingDirection(currentNode);

            if (GetDistance2D(currentNode) < 0.1f) {
                if (++currentNodeIndex >= currentPath.corners.Length) {
                    currentNodeIndex = -1;
                }
            }
        }
    }

    /// <summary>
    /// Turns to face a position.
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="overrideCurrent">Whether to override the current action if already executing one.</param>
    public void FacePosition(Vector3 position, bool overrideCurrent = false) {
        if (overrideCurrent || !isBusy) {
            currentDirection = GetFacingDirection(position);
            _isTurning = true;
        }
    }

    /// <summary>
    /// Starts moving towards a position.
    /// </summary>
    /// <param name="position">The position to move towards.</param>
    /// <param name="overrideCurrent">Whether to override the current action if already executing one.</param>
    public void MoveToPosition(Vector3 position, bool overrideCurrent = false) {
        if (overrideCurrent || !isBusy) {
            navMeshAgent.CalculatePath(position, currentPath);
            if (currentPath.corners.Length > 0) {
                currentNodeIndex = 0;
            }
        }
    }

    /// <summary>
    /// Gets a vector that faces towards a position.
    /// </summary>
    /// <returns>A vector that faces towards the specified position.</returns>
    /// <param name="position">The position to face towards.</param>
    private Vector3 GetFacingDirection(Vector3 position) {
        Vector3 direction = GetDirection2D(position - transform.position);
        direction.Normalize();
        return direction;
    }

    /// <summary>
    /// Converts a 3D vector to a 2D xz vector.
    /// </summary>
    /// <returns>The converted xz vector.</returns>
    /// <param name="vector">The 3D vector to convert.</param>
    private Vector2 GetDirection2D(Vector3 vector) {
        return new Vector2(vector.x, vector.z);
    }

    /// <summary>
    /// Gets the distance away from a position using only xz coordinates.
    /// </summary>
    /// <returns>The distance away from a position using only xz coordinates.</returns>
    /// <param name="position">The position to get a distance away from.</param>
    private float GetDistance2D(Vector3 position) {
        return Vector3.Distance(transform.position, new Vector3(position.x, transform.position.y, position.z));
    }
}