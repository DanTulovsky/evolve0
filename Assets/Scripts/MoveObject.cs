//This class handles all methods that moves the object it's attached to

using System;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private WonderingDestinationSetterRandomNode wonderingDestinationSetterRandomNode;

    //These methods will be executed by their own command

    private void Awake()
    {
        wonderingDestinationSetterRandomNode = gameObject.AddComponent<WonderingDestinationSetterRandomNode>();
    }

    public void Go()
    {
        wonderingDestinationSetterRandomNode.StartMoving();
    }

    public void RunAwayFrom(GameObject other)
    {
        wonderingDestinationSetterRandomNode.SetRandomPointAwayFrom(other);
    }

    public void SetDestination(GameObject destination)
    {
        wonderingDestinationSetterRandomNode.SetDestination(destination);
    }
    public void Stop()
    {
        wonderingDestinationSetterRandomNode.StartMoving();
    }
}