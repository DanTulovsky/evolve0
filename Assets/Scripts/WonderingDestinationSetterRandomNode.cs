using System.Collections;
using Pathfinding;
using UnityEngine;

public class WonderingDestinationSetterRandomNode : MonoBehaviour
{
    IAstarAI _ai;
    public GameObject destinationPrefab;
    private GameObject _destination;
    SpriteRenderer _destinationSpriteRenderer;

    GraphNode _randomNode;
    private bool _stopped;

    public bool Stopped => _stopped;

    // Start is called before the first frame update
    void Start()
    {
        _ai = GetComponent<IAstarAI>();

        _destination = Instantiate(destinationPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        _destinationSpriteRenderer = _destination.GetComponent<SpriteRenderer>();
        _destinationSpriteRenderer.color = GetComponent<SpriteRenderer>().color;
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

    // Set the destination in a direction away from other
    public void SetRandomPointAwayFrom(Transform me, Transform other)
    {
        GraphNode node = PickNodeAwayFrom(me, other);
        
        Vector3 point = (Vector3)node.position;
        _destination.transform.position = point;

        _ai.destination = point;
        _ai.SearchPath();
    }

    public void StopMoving(Vector3 stopPosition)
    {
        _stopped = true;
        _destination.transform.position = stopPosition;
        _ai.destination = stopPosition;
    }

    public void StartMoving()
    {
        _stopped = false;
    }

    private GraphNode PickNodeAwayFrom(Transform me, Transform other)
    {
        Vector3 position = me.position;
        Vector3 otherPosition = other.position;

        // direction away from this object
        Vector3 direction = position - otherPosition;
        // direction.Normalize();
        Vector3 move = position + direction * 4;

        // find closes node to position above
        NNConstraint constraint = NNConstraint.None;
        constraint.constrainWalkability = true;
        constraint.walkable = true;

        GraphNode node = AstarPath.active.GetNearest(move, constraint).node;
        // Debug.LogFormat("me: {0}; other: {1}; direction: {2}; move: {3}", position, otherPosition, direction,
        // move);
        return node;
    }


    // Update is called once per frame
    void Update()
    {
        // Update the destination of the AI if
        // the AI is not already calculating a path and
        // the ai has reached the end of the path or it has no path at all
        if (_ai.pathPending || (!_ai.reachedEndOfPath && _ai.hasPath)) return;

        if (!_stopped)
        {
            _ai.destination = PickRandomPoint();
        }

        _ai.SearchPath();
    }
}