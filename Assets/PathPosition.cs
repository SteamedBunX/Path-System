using UnityEngine;
public struct PathPosition
{
  public PathPosition(Vector2 position,
  float pathX,
  int nodeIndex)
  {
    this.position = position;
    this.pathX = pathX;
    this.nodeIndex = nodeIndex;
  }
  public Vector2 position;
  public float pathX;
  public int nodeIndex;
}