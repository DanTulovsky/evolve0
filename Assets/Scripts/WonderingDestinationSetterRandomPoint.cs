using Pathfinding;
using UnityEngine;

public class WonderingDestinationSetterSimple : MonoBehaviour
{
    public float radius = 1;

    IAstarAI _ai;
    private GameObject _destination;

    // Start is called before the first frame update
    void Start()
    {
        _ai = GetComponent<IAstarAI>();
        _destination = GameObject.Find("destination");
    }

    Vector3 PickRandomPoint () {
        var point = Random.insideUnitSphere * radius;

        point.z = 0;
        point += _ai.position;

        _destination.transform.position = point;

        return point;
    }
    // Update is called once per frame
    void Update()
    {
        
        // Update the destination of the AI if
        // the AI is not already calculating a path and
        // the ai has reached the end of the path or it has no path at all
        if (!_ai.pathPending && (_ai.reachedEndOfPath || !_ai.hasPath)) {
            _ai.destination = PickRandomPoint();
            _ai.SearchPath();
        }
    }
}
