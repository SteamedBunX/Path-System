using System.Collections.Generic;
using UnityEngine;

public class PathSwitchTrigger : MonoBehaviour
{
  PathSwitchTriggersController controller;
  TriggerPosition triggerPosition = TriggerPosition.TOP;

  public void setController(PathSwitchTriggersController controller)
  {
    this.controller = controller;
  }

  public void setTriggerPosition(TriggerPosition triggerPosition)
  {
    this.triggerPosition = triggerPosition;
  }

  bool isSameObject(int instanceId, GameObject other)
  {
    return instanceId == other.GetInstanceID();
  }

  void OnTriggerEnter(Collider other)
  {
    controller.gameObjectEnter(triggerPosition, other.gameObject);
  }


  void OnTriggerExit(Collider other)
  {
    controller.gameObjectExit(triggerPosition, other.gameObject);
  }
}
