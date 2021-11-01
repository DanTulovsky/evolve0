using System;
using System.Collections.Generic;
using Kryz.CharacterStats;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class Red : MonoBehaviour
{
    public GameManager.Behavior behavior;

    private bool _inCollision;
    private AIPath _ai;
    private GameSettings _gameSettings;
    private WonderingDestinationSetterRandomNode _dsetter;
    private bool _runningAway;


    public CharacterStat speedStat;
    public CharacterStat healthStat;

    private void Start()
    {
        // _gameManager = GameManager.Instance;
        _gameSettings = GameManager.Instance.gameSettings;
        _ai = GetComponent<AIPath>();
        _dsetter = GetComponent<WonderingDestinationSetterRandomNode>();
        healthStat.BaseValue = _gameSettings.baseHealth;
    }

    private void Update()
    {
        _ai.maxSpeed = speedStat.Value;

        // Check for collisions
        Collider2D selfCollider = GetComponent<Collider2D>();

        // ReSharper disable once Unity.PreferNonAllocApi
        var nearBy = new List<Collider2D>(Physics2D.OverlapCircleAll(
            transform.position,
            // Attack method checks distance between center points
            _gameSettings.attackDistance / 2,
            _gameSettings.whatIsEnemies));
        if (nearBy.Contains(selfCollider))
        {
            nearBy.Remove(selfCollider);
        }

        // Debug.LogFormat("[{0}] Enemies in range: {1}", behavior, nearBy.Count);

        if (nearBy.Count > 0)
        {
            // Focus on one enemy
            GameObject randomEnemy = nearBy[Random.Range(0, nearBy.Count - 1)].gameObject;

            switch (behavior)
            {
                case GameManager.Behavior.Dove:
                    RunAwayFrom(randomEnemy);
                    break;
                case GameManager.Behavior.Hawk:
                    Attack(randomEnemy);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            _dsetter.StartMoving();
            _runningAway = false;
        }
    }

    public bool IsDead()
    {
        return healthStat.Value <= 0;
    }

    public bool CanReproduce()
    {
        return healthStat.Value >= _gameSettings.reproduceAtHealth;
    }

    private void Attack(GameObject other)
    {
        // Debug.LogFormat("[{0}] Attacking: {1}", behavior, other.gameObject.GetComponent<Red>().behavior);
        _dsetter.StopMoving(transform.position);


        GetComponent<RedAttack>().SetTarget(other);
    }

    private void RunAwayFrom(GameObject other)
    {
        if (_runningAway) return;

        _runningAway = true;

        // Debug.LogFormat("[{0}] Running away from: {1}", behavior, other.gameObject.GetComponent<Red>().behavior);
        _dsetter.SetRandomPointAwayFrom(transform, other.transform);

        healthStat.BaseValue += _gameSettings.runAwayHealthBonus;
    }

    public void TakeDamage(float damage)
    {
        healthStat.BaseValue += damage;
    }
}