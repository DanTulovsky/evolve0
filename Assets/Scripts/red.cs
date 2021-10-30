using System;
using Kryz.CharacterStats;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Red : MonoBehaviour
{
    public GameManager.Behavior behavior;

    private const string BirdTag = "bird";
    private bool _inCollision;
    private GameManager _gameManager;
    private AIPath _ai;

    public CharacterStat speedStat;
    [SerializeField] private CharacterStat _attackDistanceStat;
    [SerializeField] private CharacterStat _healthStat;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _ai = GetComponent<AIPath>();
        _healthStat.BaseValue = GameSettings.BaseHealth;
        _attackDistanceStat.BaseValue = GameSettings.AttackDistance;
    }

    private void Update()
    {
        _ai.maxSpeed = speedStat.Value;
    }

    public bool IsDead()
    {
        return _healthStat.Value <= 0;
    }
    
    private void ProcessCollisionEnter(GameObject other)
    {
        switch (behavior)
        {
            case GameManager.Behavior.Dove:
                RunAwayFrom(other);
                break;
            case GameManager.Behavior.Hawk:
                Attack(other);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Hit(GameObject other)
    {
        Red enemy = other.GetComponent<Red>();
        
        // check how far it is and attack if close enough
        float distance = Vector3.Distance(transform.position, other.transform.position);
        if (distance <= _attackDistanceStat.Value)
        {
            Debug.LogFormat("[{0}] Hitting {1}", behavior, enemy.behavior);

            // TODO: implement this better
            enemy._healthStat.BaseValue += GameSettings.HitHealthDamage;
            if (enemy.IsDead())
            {
                _healthStat.BaseValue += GameSettings.KillHealthBonus;
            }
        }

    }

    private void Attack(GameObject other)
    {
        // TODO: This should be an interface.
        WonderingDestinationSetterRandomNode dsetter = GetComponent<WonderingDestinationSetterRandomNode>();

        Debug.LogFormat("[{0}] Attacking: {1}", behavior, other.gameObject.GetComponent<Red>().behavior);
        dsetter.StopMoving(transform.position);

        Hit(other);
    }

    private void RunAwayFrom(GameObject other)
    {
        // TODO: This should be an interface.
        WonderingDestinationSetterRandomNode dsetter = GetComponent<WonderingDestinationSetterRandomNode>();

        Debug.LogFormat("[{0}] Running away from: {1}", behavior, other.gameObject.GetComponent<Red>().behavior);
        dsetter.SetRandomPointAwayFrom(transform, other.transform);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        WonderingDestinationSetterRandomNode dsetter = GetComponent<WonderingDestinationSetterRandomNode>();
        if (dsetter.Stopped)
        {
            Debug.Log("Collision over, moving again.");
            dsetter.StartMoving();
        }

        _inCollision = false;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (_inCollision) return;

        if (other.gameObject.CompareTag(BirdTag))
        {
            _inCollision = true;
            ProcessCollisionEnter(other.gameObject);
            // Debug.LogFormat("Met other bird: {0}", other.gameObject.GetComponent<Red>().behavior);
        }
    }
}