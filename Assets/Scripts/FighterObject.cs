// This class handles all methods that allow an object it's attached to to attack

using Kryz.CharacterStats;
using UnityEngine;

public class FighterObject : MonoBehaviour
{
    private GameSettings _gameSettings;

    private Renderer rend;
    private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");

    private GameObject currentAttackTarget;
    private GameObject currentPostureTarget;
    private float _timeBtwAttack;
    private const float START_TIME_BTW_ATTACK = 0.3f; // Can attack every <value> seconds
    public bool runAway;

    [SerializeField] private CharacterStat attackDistanceStat;
    [SerializeField] private CharacterStat postureDistanceStat;

    private void Start()
    {
        _gameSettings = GameManager.instance.gameSettings;
        attackDistanceStat = new CharacterStat
        {
            BaseValue = _gameSettings.interactionDistance
        };
        postureDistanceStat = new CharacterStat
        {
            BaseValue = _gameSettings.interactionDistance
        };

        // For highlighting during attack
        rend = GetComponent<Renderer>();
        rend.material.SetFloat(OutlineEnabled, 0);
    }

    public void Attack(GameObject target)
    {
        if (currentAttackTarget != null) return;

        currentAttackTarget = target;
        rend.material.SetFloat(OutlineEnabled, 1);
    }

    public void Posture(GameObject target)
    {
        if (currentPostureTarget != null) return;

        currentPostureTarget = target;
        rend.material.SetFloat(OutlineEnabled, 1);
    }

    public GameObject CurrentAttackTarget()
    {
        return currentAttackTarget;
    }

    public GameObject CurrentPostureTarget()
    {
        return currentPostureTarget;
    }

    private void Update()
    {
        if (currentAttackTarget == null && currentPostureTarget == null)
        {
            rend.material.SetFloat(OutlineEnabled, 0);
            return;
        }

        if (_timeBtwAttack <= 0)
        {
            if (currentAttackTarget != null)
            {
                DoAttack(currentAttackTarget);
            }
            else if (currentPostureTarget != null)
            {
                if (Random.Range(0f, 1f) < _gameSettings.postureEndChance)
                {
                    Debug.Log("Had enough posturing!");
                    GetComponent<Red>().healthStat.BaseValue += _gameSettings.timeWasteHealthImpact;
                    currentPostureTarget = null;
                    runAway = true;
                    return;
                }

                DoPosture(currentPostureTarget);
            }

            _timeBtwAttack = START_TIME_BTW_ATTACK;
        }
        else
        {
            _timeBtwAttack -= Time.deltaTime;
        }
    }

    public void StopRunning()
    {
        runAway = false;
    }

    private void DoAttack(GameObject other)
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
            currentAttackTarget = null;
            rend.material.SetFloat(OutlineEnabled, 0);
        }
    }

    private void DoPosture(GameObject other)
    {
        Red me = GetComponent<Red>();
        Red enemy = other.GetComponent<Red>();

        // check how far it is and posture if close enough
        float distance = Vector3.Distance(transform.position, other.transform.position);
        if (distance <= postureDistanceStat.Value)
        {
            Debug.LogFormat("[{0}] Posturing {1}", me.behavior, enemy.behavior);
        }
        else
        {
            // Enemy ran away or died, we win
            currentPostureTarget = null;
            rend.material.SetFloat(OutlineEnabled, 0);

            me.healthStat.BaseValue += _gameSettings.winHealthImpact;
            // But we wasted time
            me.healthStat.BaseValue += _gameSettings.timeWasteHealthImpact;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, _gameSettings.interactionDistance);
    }
}