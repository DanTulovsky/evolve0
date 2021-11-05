using System;
using System.Collections.Generic;
using Kryz.CharacterStats;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class Red : MonoBehaviour
{
    public GameManager.Behavior behavior;

    private AIPath _ai;
    private GameSettings _gameSettings;

    private bool _runningAway;

    // Allows this object to move in the world
    public MoveObject moveObject;
    private FighterObject fighterObject;

    public CharacterStat speedStat;
    public CharacterStat healthStat;

    private Command setDestination;
    private Command stop;

    private void Awake()
    {
        // _gameManager = GameManager.Instance;
        _gameSettings = GameManager.Instance.gameSettings;
        _ai = GetComponent<AIPath>();
        moveObject = gameObject.AddComponent<MoveObject>();
        fighterObject = gameObject.AddComponent<FighterObject>();
        healthStat.BaseValue = _gameSettings.baseHealth;
    }

    private void Update()
    {
        _ai.maxSpeed = speedStat.Value;

        var nearBy = GetNearby();

        if (nearBy.Count > 0)
        {
            // Focus on one enemy
            GameObject randomEnemy = nearBy[Random.Range(0, nearBy.Count - 1)].gameObject;

            switch (behavior)
            {
                case GameManager.Behavior.Dove:
                    DoveHandler(randomEnemy);
                    break;
                case GameManager.Behavior.Hawk:
                    HawkHandler(randomEnemy);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            ExecuteNewCommand(new GoCommand(moveObject));
            _runningAway = false;
        }
    }

    //Will execute the command and do stuff to the list to make the replay, undo, redo system work
    private static void ExecuteNewCommand(Command command)
    {
        command.Execute();

        //Add the new command to the last position in the list
        // undoCommands.Push(commandButton);

        //Remove all redo commands because redo is not defined when we have add a new command
        // redoCommands.Clear();
    }

    private List<Collider2D> GetNearby()
    {
        // Check for collisions
        Collider2D selfCollider = GetComponent<Collider2D>();

        // ReSharper disable once Unity.PreferNonAllocApi
        var nearBy = new List<Collider2D>(Physics2D.OverlapCircleAll(
            transform.position,
            // Attack method checks distance between center points
            _gameSettings.interactionDistance / 2,
            _gameSettings.whatIsEnemies));

        if (nearBy.Contains(selfCollider))
        {
            nearBy.Remove(selfCollider);
        }

        return nearBy;
    }

    private void HawkHandler(GameObject other)
    {
        switch (other.GetComponent<Red>().behavior)
        {
            default:
                if (healthStat.Value <= _gameSettings.runAwayAtHealth)
                {
                    // Keep attacking the same target if you have one
                    RunAwayFrom(fighterObject.CurrentAttackTarget() != null ? fighterObject.CurrentAttackTarget() : other);
                }
                else
                {
                    Attack(other);
                }

                break;
        }
    }

    private void DoveHandler(GameObject other)
    {
        switch (other.GetComponent<Red>().behavior)
        {
            case GameManager.Behavior.Hawk:
                // Runs away even if currently posturing
                RunAwayFrom(other);
                break;
            case GameManager.Behavior.Dove:
                if (fighterObject.runAway)
                {
                    if (fighterObject.CurrentPostureTarget() != null)
                    {
                        RunAwayFrom(fighterObject.CurrentPostureTarget());
                        Invoke(nameof(fighterObject.StopRunning), 3f);
                    }
                }
                else
                {
                    // Keep posturing with the same target if you have one
                    Posture(fighterObject.CurrentPostureTarget() != null
                        ? fighterObject.CurrentPostureTarget()
                        : other);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Don't fight, but posture until someone backs down
    private void Posture(GameObject other)
    {
        ExecuteNewCommand(new SetDestinationCommand(moveObject, other));
        ExecuteNewCommand(new PostureCommand(fighterObject, other));
    }

    public bool IsDead()
    {
        return healthStat.Value <= 0;
    }

    public bool CanReproduce()
    {
        return _gameSettings.enableReproduction && healthStat.Value >= _gameSettings.reproduceAtHealth;
    }

    public bool HasTarget()
    {
        return fighterObject.CurrentAttackTarget() != null ||
               fighterObject.CurrentPostureTarget() != null;
    }

    private void Attack(GameObject other)
    {
        ExecuteNewCommand(new SetDestinationCommand(moveObject, other));
        ExecuteNewCommand(new AttackCommand(fighterObject, other));
    }

    private void RunAwayFrom(GameObject other)
    {
        if (_runningAway) return;

        _runningAway = true;

        ExecuteNewCommand(new RunAwayCommand(moveObject, other));

        healthStat.BaseValue += _gameSettings.loseHealthImpact;
    }

    public void TakeDamage(float damage)
    {
        healthStat.BaseValue += damage;
    }
}