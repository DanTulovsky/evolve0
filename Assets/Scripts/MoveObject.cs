// This class handles all methods that moves the object it's attached to

using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveObject : MonoBehaviour
{
    private IAstarAI _ai;

    // Remaining distance to the destination
    private float _lastPathRemainingDistance;

    // Time at which the above distance was calculated
    private float _lastPathRemainingDistanceTime;

    // How long it's ok to remain in the same place without progress
    private float _lastPathStoppedToleranceTime;

    private GraphNode _randomNode;
    private bool stuck;
    [SerializeField] private float stuckTime;


    //These methods will be executed by their own command

    private void Awake()
    {
        _ai = GetComponent<IAstarAI>();

        _lastPathRemainingDistanceTime = Time.time;
        _lastPathRemainingDistance = float.MaxValue;
        _lastPathStoppedToleranceTime = 2.0f;
    }

    // Go allows the object to wonder
    public void Go()
    {
        _ai.canMove = true;
    }

    // RunAwayFrom makes the object go in the opposite direction from other
    public void RunAwayFrom(GameObject other)
    {
        GraphNode node = PickNodeAwayFrom(gameObject.transform, other.transform);

        Vector3 point = (Vector3)node.position;

        _ai.destination = point;
        _ai.SearchPath();
    }

    // SetDestination sets the destination for the object to move
    public void SetDestination(GameObject destination)
    {
        _ai.destination = destination.transform.position;
    }

    public void Stop()
    {
        _ai.canMove = false;
    }

    private Vector3 PickRandomPoint()
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
        return point;
    }


    private static GraphNode PickNodeAwayFrom(Transform me, Transform other)
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
        return node;
    }

    private bool MovementProgressMade()
    {
        // Haven't reach the end, and we have a path
        if (!_ai.reachedEndOfPath && _ai.hasPath && !float.IsPositiveInfinity(_ai.remainingDistance))
        {
            if (_ai.remainingDistance < _lastPathRemainingDistance)
            {
                // We made progress, do nothing
                _lastPathRemainingDistance = _ai.remainingDistance;
                _lastPathRemainingDistanceTime = Time.time;
                return true;
            }
            else
            {
                stuckTime = Time.time - _lastPathRemainingDistanceTime;
                if (stuckTime > _lastPathStoppedToleranceTime)
                {
                    // If we are fighting, this is ok
                    if (gameObject.GetComponent<Red>().HasTarget())
                    {
                        return true;
                    }
                    Debug.LogFormat("Stuck for {0} out of {1}", Time.time - _lastPathRemainingDistanceTime, _lastPathStoppedToleranceTime);
                    return false;
                }
            }
        }

        return true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_ai.pathPending) return;

        // check if we've made any progress
        if (!MovementProgressMade())
        {
            stuck = true;
        }

        // Update the destination of the AI if
        // the AI is not already calculating a path and
        // the ai has reached the end of the path or it has no path at all
        if (!_ai.reachedEndOfPath && _ai.hasPath)
        {
            if (!stuck) return;
        }

        _ai.destination = PickRandomPoint();

        _ai.SearchPath();
        stuck = false;
        _lastPathRemainingDistanceTime = Time.time;
        _lastPathRemainingDistance = float.MaxValue;
    }
}