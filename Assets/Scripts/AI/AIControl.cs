using UnityEngine;

namespace Bardmages.AI {
    /// <summary>
    /// Allows a player to move a bardmage.
    /// </summary>
    class AIControl : BaseControl {

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
        /// <summary> Whether the bardmage is currently turning to face a position. </summary>
        public bool isTurning {
            get { return _isTurning; }
        }
        /// <summary> The tolerance for turning towards an angle. </summary>
        private const float ANGLE_TOLERANCE = 5f;

        /// <summary> Whether the bardmage is currently executing an action. </summary>
        internal bool isBusy {
            get { return isMoving || _isTurning; }
        }

        /// <summary> The radius or the bardmage's capsule collider. </summary>
        private float radius;
        /// <summary> The character controller controlling the bardmage's movement. </summary>
        private CharacterController characterController;

        /// <summary>
        /// Use this for initialization.
        /// </summary>
        protected override void Start() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
            currentPath = new NavMeshPath();
            characterController = GetComponent<CharacterController>();
            radius = characterController.radius;
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
            if (_isTurning && GetAngleDifference(currentDirection) < ANGLE_TOLERANCE) {
                _isTurning = false;
                currentDirection = Vector3.zero;
            } else if (isMoving) {
                bool groundInFront = false;
                Vector3 front = transform.position + transform.forward * radius * 2;
                RaycastHit hit;
                if (Physics.Raycast(front, Vector3.down, out hit, 50)) {
                    groundInFront = hit.collider.tag != "Kill" && hit.collider.tag != "Player";
                }

                // Attempt to not walk off the map.
                if (groundInFront) {
                    currentDirection = GetFacingDirection(currentNode);
                } else if (Vector3.Magnitude(characterController.velocity) > speed / 2) {
                    currentDirection = -GetFacingDirection(currentNode);
                } else {
                    currentDirection = Vector3.zero;
                }

                if (GetDistance2D(currentNode) < 0.1f) {
                    if (++currentNodeIndex >= currentPath.corners.Length) {
                        currentNodeIndex = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the difference between the current angle and another angle.
        /// </summary>
        /// <param name="otherDirection">The direction to compare against.</param>
        private float GetAngleDifference(Vector2 otherDirection) {
            return Vector3.Angle(VectorUtil.GetDirection2D(transform.forward), otherDirection);
        }

        /// <summary>
        /// Turns to face a position.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="overrideCurrent">Whether to override the current action if already executing one.</param>
        internal void FacePosition(Vector3 position, bool overrideCurrent = false) {
            if (overrideCurrent || !isBusy) {
                Vector2 targetDirection = GetFacingDirection(position);
                if (GetAngleDifference(targetDirection) > ANGLE_TOLERANCE) {
                    currentDirection = targetDirection;
                    _isTurning = true;
                } else {
                    currentDirection = Vector2.zero;
                }
            }
        }

        /// <summary>
        /// Starts moving towards a position.
        /// </summary>
        /// <param name="position">The position to move towards.</param>
        /// <param name="overrideCurrent">Whether to override the current action if already executing one.</param>
        internal void MoveToPosition(Vector3 position, bool overrideCurrent = false) {
            if (navMeshAgent != null && navMeshAgent.isOnNavMesh && (overrideCurrent || !isBusy)) {
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
            Vector3 direction = VectorUtil.GetDirection2D(position - transform.position);
            direction.Normalize();
            return direction;
        }

        /// <summary>
        /// Gets the distance away from a position using only xz coordinates.
        /// </summary>
        /// <returns>The distance away from a position using only xz coordinates.</returns>
        /// <param name="position">The position to get a distance away from.</param>
        internal float GetDistance2D(Vector3 position) {
            return Vector3.Distance(transform.position, new Vector3(position.x, transform.position.y, position.z));
        }
    }
}