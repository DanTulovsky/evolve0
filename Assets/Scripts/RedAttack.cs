using Kryz.CharacterStats;
using UnityEngine;

public class RedAttack : MonoBehaviour
{
    private float _timeBtwAttack;
    private  GameSettings _gameSettings;
    [SerializeField] private CharacterStat attackDistanceStat;

    // Can attack every <value> seconds
    public float startTimeBtwAttack = 0.3f;
    [SerializeField] private GameObject attackTarget;

    public void SetTarget(GameObject target)
    {
        attackTarget = target;
    }

    public GameObject AttackTarget()
    {
        return attackTarget;
    }
    private void Awake()
    {
        _gameSettings = GameManager.Instance.gameSettings;
        attackDistanceStat.BaseValue = _gameSettings.interactionDistance;
    }

    private void Update()
    {
        if (attackTarget == null) return;

        if (_timeBtwAttack <= 0)
        {
            Hit(attackTarget);
            _timeBtwAttack = startTimeBtwAttack;
        }
        else
        {
            _timeBtwAttack -= Time.deltaTime;
        }
    }

    private void Hit(GameObject other)
    {
        Red me = GetComponent<Red>();
        Red enemy = other.GetComponent<Red>();

        // check how far it is and attack if close enough
        float distance = Vector3.Distance(transform.position, other.transform.position);
        if (distance <= attackDistanceStat.Value)
        {
            // Debug.LogFormat("[{0}] Hitting {1}", me.behavior, enemy.behavior);
            enemy.TakeDamage(_gameSettings.hitHealthDamage);

            if (enemy.IsDead())
            {
                me.healthStat.BaseValue += _gameSettings.winHealthImpact;
            }
        }
        else
        {
            attackTarget = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, _gameSettings.interactionDistance);
    }
}