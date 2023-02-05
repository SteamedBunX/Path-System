using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Pathed2_5DCharacterController : MonoBehaviour {
  public Path path;
  public CharacterController playerRigidBody;
  int currentPathInstanceId;
  int currentNodeIndex;
  PlayerInput inputControl;
  Pathed2_5DMovementController bmc;
  int groundableLayerMask;
  public LayerMask oneWayPlatformLayer;
  public LayerMask groundLayer;
  public Transform LookAt;
  Direction _facing = Direction.RIGHT;
  Direction facing {
    get {
      return _facing;
    }
    set {
      _facing = value;
      updateGlobalRotation();
    }
  }
  float _pathRotation = 0;
  float pathRotation {
    get { return _pathRotation; }
    set {
      _pathRotation = value;
      updateGlobalRotation();
    }
  }

  void Start() {
    setupInputControl();
    groundableLayerMask = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("OneWayPlatform");
    bmc = new Pathed2_5DMovementController(playerRigidBody, path, 10f, 2.5f, 10f, 10f);
    bmc.snapToPath(0);
  }

  void setupInputControl() {
    inputControl = new PlayerInput();
    inputControl.Combat.Enable();
    inputControl.Combat.JumpInput.performed += JumpInput;
    inputControl.Combat.Down.performed += DownInput;
  }

  // Update is called once per frame
  void Update() {
    if (inputControl.Combat.Right.ReadValue<float>() > 0.1f) {
      turn(Direction.RIGHT);
      bmc.setXVelocity(12f);
    } else if (inputControl.Combat.Left.ReadValue<float>() > 0.1f) {
      turn(Direction.LEFT);
      bmc.setXVelocity(-12f);
    } else {
      bmc.setXVelocity(0f);
    }
    bmc.update(Time.deltaTime, isGrounded());
    checkPathDirection(bmc.currentNodePath);
  }

  void turn(Direction direction) {
    if (facing != direction) {
      facing = direction;
    }
  }

  void checkPathDirection(NodePath nodePath) {
    Debug.Log($"Current rotation {pathRotation}, New rotation: {nodePath.rotation}");
    if (nodePath.rotation != pathRotation) {
      pathRotation = nodePath.rotation;
    }
  }

  public void updateGlobalRotation() {
    updateYRotation(
      pathRotation + (facing == Direction.RIGHT ? 0 : 180)
    );
    LookAt.transform.localEulerAngles = new Vector3(0, facing == Direction.RIGHT ? 0 : 180, 0);
  }

  public void updateYRotation(float newYRotation) {
    transform.eulerAngles = new Vector3(0, newYRotation, 0);
  }

  void JumpInput(InputAction.CallbackContext context) {
    if (isGrounded() && playerRigidBody.velocity.y <= 0f) {
      bmc.setYVelocity(10f);
    }
  }

  void DownInput(InputAction.CallbackContext context) {
    Collider onwayPlatformCollider;
    if (isOnOneWayPlatform(out onwayPlatformCollider)) {
      Physics.IgnoreCollision(playerRigidBody, onwayPlatformCollider, true);
    }
  }

  public void setPath(Path newPath, int nodeIndex) {
    if (path.gameObject.GetInstanceID() != newPath.gameObject.GetInstanceID()) {
      path = newPath;
      currentNodeIndex = nodeIndex;
      bmc.setPath(newPath, nodeIndex);
    }
  }

  void OnDrawGizmos() {
    RaycastHit groundHitInfo;
    Vector3 groundCheckOrigin = this.transform.position +
        Vector3.up * (playerRigidBody.radius);
    Physics.SphereCast(groundCheckOrigin, playerRigidBody.radius, Vector3.down, out groundHitInfo,
     0.1f, groundableLayerMask);
    Gizmos.color = groundHitInfo.collider == null ? Color.green : Color.red;
    Gizmos.DrawSphere(groundCheckOrigin, playerRigidBody.radius);
  }

  bool isGrounded() {
    if (bmc.velocityY > 0f) {
      return false;
    }
    RaycastHit groundHitInfo;
    Vector3 groundCheckOrigin = this.transform.position +
      Vector3.up * (playerRigidBody.radius);
    Physics.SphereCast(groundCheckOrigin, playerRigidBody.radius,
    Vector3.down, out groundHitInfo,
         0.1f, groundableLayerMask);
    return groundHitInfo.collider != null;
  }

  bool isOnOneWayPlatform(out Collider oneWayPlatformCollider) {
    RaycastHit groundHitInfo;
    Vector3 groundCheckOrigin = this.transform.position +
      Vector3.up * (playerRigidBody.radius);
    Physics.SphereCast(groundCheckOrigin, playerRigidBody.radius,
    Vector3.down, out groundHitInfo,
         0.1f, oneWayPlatformLayer);
    oneWayPlatformCollider = groundHitInfo.collider;
    return groundHitInfo.collider;
  }

}

public enum Direction {
  RIGHT = 1,
  LEFT = -1
}
