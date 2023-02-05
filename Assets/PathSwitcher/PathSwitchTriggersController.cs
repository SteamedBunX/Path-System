using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSwitchTriggersController : MonoBehaviour
{
  public GameObject topTrigger;
  public Path topPath;
  public int topPathNode;
  public GameObject bottomTrigger;
  public Path bottomPath;
  public int bottomPathNode;
  List<int> colliderInTopTrigger = new List<int>();
  List<int> colliderInBottomTrigger = new List<int>();

  void Awake()
  {
    PathSwitchTrigger topTriggerScript = topTrigger.AddComponent<PathSwitchTrigger>();
    topTriggerScript.setController(this);
    topTriggerScript.setTriggerPosition(TriggerPosition.TOP);
    PathSwitchTrigger bottomTriggerScript = bottomTrigger.AddComponent<PathSwitchTrigger>();
    bottomTriggerScript.setController(this);
    bottomTriggerScript.setTriggerPosition(TriggerPosition.BOTTOM);
  }

  public void gameObjectEnter(TriggerPosition triggerPosition, GameObject go)
  {
    switch (triggerPosition)
    {
      case TriggerPosition.TOP:
        colliderInTopTrigger.Add(go.GetInstanceID());
        break;
      case TriggerPosition.BOTTOM:
        colliderInBottomTrigger.Add(go.GetInstanceID());
        break;
    }
    updatePathForObject(go);
  }

  public void gameObjectExit(TriggerPosition triggerPosition, GameObject go)
  {
    switch (triggerPosition)
    {
      case TriggerPosition.TOP:
        colliderInTopTrigger.Remove(go.GetInstanceID());
        break;
      case TriggerPosition.BOTTOM:
        colliderInBottomTrigger.Remove(go.GetInstanceID());
        break;
    }
    updatePathForObject(go);
  }

  public void updatePathForObject(GameObject go)
  {
    bool inTop = colliderInTopTrigger.Contains(go.GetInstanceID());
    bool inBottom = colliderInBottomTrigger.Contains(go.GetInstanceID());
    if (inBottom)
    {
      switchPath(go.GetComponent<Pathed2_5DCharacterController>(), bottomPath, bottomPathNode);
    }
    else
    {
      if (inTop)
      {
        switchPath(go.GetComponent<Pathed2_5DCharacterController>(), topPath, topPathNode);
      }
    }
  }

  public void switchPath(Pathed2_5DCharacterController character, Path path, int nodeIndex)
  {
    character.setPath(path, nodeIndex);
  }
}

public enum TriggerPosition
{
  TOP,
  BOTTOM
}
