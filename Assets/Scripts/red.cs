using System.Collections;
using System.Collections.Generic;
using Kryz.CharacterStats;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

public class red : MonoBehaviour
{
    public CharacterStat speedStat;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Mapped to BehaviorTree blackboard
        // speed = speedStat.Value;

    }
}
