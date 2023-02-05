using System;
using UnityEngine;

public class Pathed2_5DMovementController
{
  public float velocityX = 0;
  public float velocityY = 0;
  Vector2 groundCordinate;
  float timeDelta = 0;
  public float gravityAscend;
  public float gravityDescend;
  public float horizontalDecelerationRate;
  public float airHorizontalDecelerationRate;
  public bool useGravity = true;
  public bool useXDeceleration = true;
  public bool ignoreGravityThisFrame = false;
  public bool ignoreXDecelerationThisFrame = false;
  public bool useAirXDecelerationThisFrame = false;
  bool isGrounded = false;
  public NodePath currentNodePath
  {
    get
    {
      return path.nodePaths[nodeIndex];
    }
  }
  CharacterController rigidBody;
  Path path;
  int nodeIndex;

  public Pathed2_5DMovementController(
      CharacterController rigidBody,
      Path path,
      float gravity = 1f,
      float gravityDescendRatio = 1f,
      float horizontalDecelerationRate = 1f,
      float airHorizontalDecelerationRate = 1f
      )
  {
    this.path = path;
    gravityAscend = gravity;
    gravityDescend = gravity * gravityDescendRatio;
    this.horizontalDecelerationRate = horizontalDecelerationRate;
    this.airHorizontalDecelerationRate = airHorizontalDecelerationRate;
    this.rigidBody = rigidBody;
    groundCordinate = new Vector2(rigidBody.bounds.center.x, rigidBody.bounds.center.z);
  }

  public void snapToPath(int pathNodeIndex)
  {
    nodeIndex = pathNodeIndex;
    groundCordinate = path.findClosestPlayerPositionForNode(groundCordinate, pathNodeIndex);
    MovePosition(new Vector3(groundCordinate.x, rigidBody.bounds.center.y, groundCordinate.y));
  }

  public void update(float timeDelta, bool isGrounded)
  {
    this.isGrounded = isGrounded;
    this.timeDelta = timeDelta;
    groundCordinate = new Vector2(rigidBody.bounds.center.x, rigidBody.bounds.center.z);
    movementX();
    movementY();
    ignoreGravityThisFrame = false;
    ignoreXDecelerationThisFrame = false;
    useAirXDecelerationThisFrame = false;
  }

  public void MovePosition(Vector3 newPosition)
  {

    Vector3 positionDelta = newPosition - rigidBody.bounds.center;
    rigidBody.Move(positionDelta);
  }

  public void movementX()
  {
    (nodeIndex, groundCordinate) = path.Move(nodeIndex, groundCordinate, velocityX * timeDelta);
    MovePosition(new Vector3(groundCordinate.x, rigidBody.bounds.center.y, groundCordinate.y));
  }

  public void movementY()
  {
    if (!useGravity || ignoreGravityThisFrame)
    {

    }
    else
    {
      if (isGrounded && velocityY < 0)
      {
        setYVelocity(-1f);
        MovePosition(new Vector3(rigidBody.bounds.center.x, rigidBody.bounds.center.y + -1f * timeDelta, rigidBody.bounds.center.z));
      }
      else
      {
        float newY = velocityY - gravityAscend * timeDelta;
        float dy = (newY + velocityY) / 2 * timeDelta;
        velocityY = newY;
        MovePosition(new Vector3(rigidBody.bounds.center.x, rigidBody.bounds.center.y + dy, rigidBody.bounds.center.z));
      }
    }
  }

  public void setPath(Path path)
  {
    this.path = path;
    // TODO: find the cloest point that can be snapped to
    // Check the closest dot
    // Calculate the distance of both potision and then snap to the closer one.
  }

  public void setPath(Path path, int nodeIndex)
  {
    this.path = path;
    this.nodeIndex = nodeIndex;
  }

  public void applyXAcceleration(float x, float timeDelta)
  {
    float newX = velocityX + x * timeDelta;
    setXVelocity(newX);
  }

  public void setXVelocity(float x)
  {
    velocityX = x;
  }

  public void setYVelocity(float y)
  {
    velocityY = y;
  }

  public void setVelocity(float x, float y)
  {
    velocityX = x;
    velocityY = y; ;
  }

  void applyHorizontalDeceleration()
  {
    if (velocityX != 0 && useXDeceleration && !ignoreXDecelerationThisFrame)
    {
      float newX = (Math.Abs(velocityX)
       - (useAirXDecelerationThisFrame ? airHorizontalDecelerationRate : horizontalDecelerationRate) * timeDelta) *
       (velocityX > 0 ? 1 : -1);
      if (isSameSideNumber(velocityX, newX))
      {
        setXVelocity(newX);
      }
      else
      {
        setXVelocity(0);
      }
    }
  }

  bool isSameSideNumber(float num1, float num2)
  {
    return num1 * num2 >= 0;
  }
}