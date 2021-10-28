using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class WonderingDestinationSetterRandomNode : MonoBehaviour
{
    IAstarAI _ai;
    private GameObject _destination;
    GraphNode _randomNode;

    // Start is called before the first frame update
    void Start()
    {
        _ai = GetComponent<IAstarAI>();
        _destination = GameObject.Find("destination");
    }

    Vector3 PickRandomPoint()
    {
        GridGraph grid = AstarPath.active.data.gridGraph;

        while (true)
        {
            _randomNode = grid.nodes[Random.Range(0, grid.nodes.Length)];

            // Not walkable
            if (!_randomNode.Walkable) continue;

            // Unreachable
            GraphNode aiNode = AstarPath.active.GetNearest(_ai.position, NNConstraint.Default).node;
            if (!PathUtilities.IsPathPossible(aiNode, _randomNode)) continue;

            // Valid
            break;
        }

        Vector3 point = (Vector3)_randomNode.position;
        _destination.transform.position = point;
        return point;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the destination of the AI if
        // the AI is not already calculating a path and
        // the ai has reached the end of the path or it has no path at all
        if (!_ai.pathPending && (_ai.reachedEndOfPath || !_ai.hasPath))
        {
            _ai.destination = PickRandomPoint();
            _ai.SearchPath();
        }
    }
}