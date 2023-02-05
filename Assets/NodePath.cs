using UnityEngine;

public struct NodePath
{
  public float rotation;
  public Vector2 direction;
  public Vector2 origin;
  public float Length;

  public NodePath(Vector2 direction, Vector2 orginPoint, float Length, float rotation)
  {
    this.direction = direction;
    this.origin = orginPoint;
    this.Length = Length;
    this.rotation = rotation;
  }
  public Vector2 pointOnPath(float pointX)
  {
    return new Vector2(origin.x + direction.x * pointX, origin.y + direction.y * pointX);
  }

  public Vector2 pointOnPathFromEnd(float pointX)
  {
    return new Vector2(origin.x + direction.x * (pointX + Length), origin.y + direction.y * (pointX + Length));
  }

  public Vector2 end()
  {
    return pointOnPath(Length);
  }

  public float findPointOfPathNode(Vector2 point)
  {
    return getDistanceBetweenPoints(point, origin);
  }

  float getDistanceBetweenPoints(Vector2 p1, Vector2 p2)
  {
    return Mathf.Abs((p2 - p1).magnitude);
  }
}
