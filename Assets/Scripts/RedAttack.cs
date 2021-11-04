using Kryz.CharacterStats;
using UnityEngine;

public class RedAttack : MonoBehaviour
{
    private float _timeBtwAttack;
    private GameSettings _gameSettings;
    private Renderer rend;
    [SerializeField] private CharacterStat attackDistanceStat;

    // Can attack every <value> seconds
    public float startTimeBtwAttack = 0.3f;
    [SerializeField] private GameObject attackTarget;
    private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");
    private WonderingDestinationSetterRandomNode _dsetter;

    public void SetTarget(GameObject target)
    {
        if (attackTarget != null) return;

        attackTarget = target;
        _dsetter.SetDestination(target);
        rend.sharedMaterial.SetFloat(OutlineEnabled, 1);
    }

    public GameObject GetAttackTarget()
    {
        return attackTarget;
    }

    private void Awake()
    {
        _gameSettings = GameManager.Instance.gameSettings;
        _dsetter = GetComponent<WonderingDestinationSetterRandomNode>();
        attackDistanceStat.BaseValue = _gameSettings.interactionDistance;
        rend = GetComponent<Renderer>();
        rend.sharedMaterial.SetFloat(OutlineEnabled, 0);
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
            rend.sharedMaterial.SetFloat(OutlineEnabled, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, _gameSettings.interactionDistance);
    }
}