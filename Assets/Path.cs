using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
  public List<Transform> nodes;
  public List<NodePath> nodePaths;
  public bool looped;
  // Start is called before the first frame update
  void Awake()
  {
    Transform[] allTransform = GetComponentsInChildren<Transform>();
    List<Transform> nodes = new List<Transform>();
    var childrenIndex = 0;
    for (int i = 0; i < allTransform.Length; i++)
    {
      if (allTransform[i].GetInstanceID() != transform.GetInstanceID())
      {
        nodes.Add(allTransform[i]);
        childrenIndex++;
      }
    }
    this.nodes = nodes;
    loadNodeInfos();
  }

  void loadNodeInfos()
  {
    nodePaths = new List<NodePath>();
    for (int i = 0; i < nodes.Count; i++)
    {
      int di = i == nodes.Count - 1 ? 0 : i + 1;
      float dx = nodes[di].position.x - nodes[i].position.x;
      float dy = nodes[di].position.z - nodes[i].position.z;
      float pathLength = (float)Math.Sqrt(dy * dy + dx * dx);
      Vector2 direction = new Vector2(dx / pathLength, dy / pathLength);
      Vector2 origin = new Vector2(nodes[i].position.x, nodes[i].position.z);
      float rotation = -Vector2.SignedAngle(new Vector2(1, 0), direction);
      Debug.Log(direction);
      nodePaths.Add(new NodePath(direction, origin, pathLength, rotation));
    }
  }

  public Vector2 findClosestPlayerPositionForPath(Vector3 playerPosition, int pointIndex)
  {
    return findClosestPlayerPositionForNode(new Vector2(playerPosition.x, playerPosition.z), pointIndex);
  }

  public float findPointOfPathNode(Vector2 point, int nodeIndex)
  {
    return nodePaths[nodeIndex].findPointOfPathNode(point);
  }

  public (int, Vector2) Move(int nodeIndex, Vector2 beginPosition, float dx)
  {
    float pointX = findPointOfPathNode(beginPosition, nodeIndex);
    return Move(nodeIndex, pointX, dx);
  }

  public (int, Vector2) Move(int nodeIndex, float x, float dx)
  {
    dx = dx + x;
    if (nodePaths[nodeIndex].Length >= dx && dx > 0)
    {
      return (nodeIndex, nodePaths[nodeIndex].pointOnPath(dx));
    }
    else
    {
      if (dx > 0)
      {
        while (dx > nodePaths[nodeIndex].Length)
        {
          dx -= nodePaths[nodeIndex].Length;
          nodeIndex++;
          if (!looped && nodeIndex >= nodePaths.Count - 1)
          {
            nodeIndex = nodePaths.Count - 2;
            return (nodePaths.Count - 2, nodePaths[nodePaths.Count - 2].end());
          }
          else if (nodeIndex >= nodePaths.Count)
          {
            nodeIndex = 0;
          }
        }
        return (nodeIndex, nodePaths[nodeIndex].pointOnPath(dx));
      }
      else
      {
        nodeIndex--;
        if (!looped && nodeIndex < 0)
        {
          nodeIndex = 0;
          return (0, nodePaths[0].origin);
        }
        if (nodeIndex < 0)
        {
          nodeIndex = nodePaths.Count - 1;
        }
        while (-dx > nodePaths[nodeIndex].Length)
        {
          dx += nodePaths[nodeIndex].Length;
          nodeIndex--;
          if (!looped && nodeIndex < 0)
          {
            nodeIndex = 0;
            return (0, nodePaths[0].origin);
          }
          else if (nodeIndex < 0)
          {
            nodeIndex = nodePaths.Count - 1;
          }
        }
      }
      return (nodeIndex, nodePaths[nodeIndex].pointOnPathFromEnd(dx));
    }
  }

  public Vector2 findClosestPlayerPositionForNode(Vector2 playerPosition, int pointIndex)
  {
    if (nodes.Count < pointIndex + 1)
    {
      return Vector2.positiveInfinity;
    }
    if (nodes[pointIndex].position.x == nodes[pointIndex + 1].position.x)
    {
      return new Vector2(nodes[pointIndex].position.x, playerPosition.y);
    }
    if (nodes[pointIndex].position.y == nodes[pointIndex + 1].position.y)
    {
      return new Vector2(playerPosition.x, nodes[pointIndex].position.y);
    }

    float x1 = nodes[pointIndex].position.x;
    float y1 = nodes[pointIndex].position.z;
    float x2 = nodes[pointIndex + 1].position.x;
    float y2 = nodes[pointIndex + 1].position.z;

    // Line AB represented as a1x + b1y = c1 
    float a1 = y2 - y1;
    float b1 = x1 - x2;
    float c1 = a1 * (x1) + b1 * (y1);

    float x3 = playerPosition.x;
    float y3 = playerPosition.y;

    float prepSlope = -(x2 - x1) / (y2 - y1);
    float m = y3 - x3 * prepSlope;

    float x4, y4;
    if (prepSlope == 0)
    {
      x4 = x3 + 1;
      y4 = x4 * prepSlope + m;
    }
    else
    {
      y4 = y3 + 1;
      x4 = (y4 - m) / prepSlope;
    }

    // Line CD represented as a2x + b2y = c2 
    float a2 = y4 - y3;
    float b2 = x3 - x4;
    float c2 = a2 * (x3) + b2 * (y3);

    float determinant = a1 * b2 - a2 * b1;

    float x = (b2 * c1 - b1 * c2) / determinant;
    float y = (a1 * c2 - a2 * c1) / determinant;
    return new Vector2(x, y);
  }

  public (int, Vector2) findBestPositionToSnipeTo(Vector3 playerPosition)
  {
    return findBestPositionToSnipeTo(new Vector2(playerPosition.x, playerPosition.z));
  }

  public (int, Vector2) findBestPositionToSnipeTo(Vector2 playerPosition)
  {
    int bestNodeIndex = 0;
    float bestDistanceToNodeSquare = -1f;
    foreach (var node in nodes.Select((value, index) => new { value, index }))
    {
      float xDiff = node.value.transform.position.x - playerPosition.x;
      float yDiff = node.value.transform.position.z - playerPosition.y;
      float distanceSquare = xDiff * xDiff + yDiff * yDiff;
      if (distanceSquare < bestDistanceToNodeSquare || bestDistanceToNodeSquare == -1f)
      {
        bestDistanceToNodeSquare = distanceSquare;
        bestNodeIndex = node.index;
      }
    }
    Vector2 snapPosition = findClosestPlayerPositionForNode(playerPosition, bestNodeIndex);
    if (looped || bestNodeIndex != 0)
    {
      int nodeToTest = bestNodeIndex == 0 ? nodes.Count - 1 : bestNodeIndex - 1;
      Vector2 snapPositionPathBefore = findClosestPlayerPositionForNode(playerPosition, nodeToTest);
      if (getDistanceBetweenPoints(snapPositionPathBefore, playerPosition) < getDistanceBetweenPoints(snapPositionPathBefore, playerPosition))
      {
        bestNodeIndex = nodeToTest;
        snapPosition = snapPositionPathBefore;
      }
    }
    return (bestNodeIndex, snapPosition);
  }

  public float getDistanceBetweenPoints(Vector2 p1, Vector2 p2)
  {
    if (p2.y == p1.y)
    {
      return p1.x;
    }
    if (p2.x == p1.x)
    {
      return p1.y;
    }
    float dx = p2.x - p1.x;
    float dy = p2.y - p1.y;
    return (float)Math.Sqrt(dx * dx + dy * dy);
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.green;
    Transform[] allTransform = GetComponentsInChildren<Transform>();
    List<Transform> nodes = new List<Transform>();
    for (int i = 0; i < allTransform.Length; i++)
    {
      if (allTransform[i].GetInstanceID() != transform.GetInstanceID())
      {
        Gizmos.DrawWireSphere(allTransform[i].position, .5f);
        nodes.Add(allTransform[i]);
      }
    }
    if (nodes.Count > 1)
    {
      for (int i = 0; i < nodes.Count - 1; i++)
      {
        Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
      }
      if (nodes.Count > 2 && looped)
      {
        Gizmos.DrawLine(nodes[nodes.Count - 1].position, nodes[0].position);
      }
    }
  }
}
