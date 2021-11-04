// This class handles all methods that allow an object it's attached to to attack

using Kryz.CharacterStats;
using UnityEngine;

public class FighterObject : MonoBehaviour
{
    private GameSettings _gameSettings;

    private Renderer rend;
    private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");

    private GameObject currentTarget;
    private float _timeBtwAttack;
    public float startTimeBtwAttack = 0.3f; // Can attack every <value> seconds
    [SerializeField] private CharacterStat attackDistanceStat;

    private void Start()
    {
        _gameSettings = GameManager.Instance.gameSettings;
        attackDistanceStat = new CharacterStat
        {
            BaseValue = _gameSettings.interactionDistance
        };

        // For highlighting during attack
        rend = GetComponent<Renderer>();
        rend.sharedMaterial.SetFloat(OutlineEnabled, 0);
    }

    public void Attack(GameObject target)
    {
        if (currentTarget != null) return;

        currentTarget = target;
        rend.sharedMaterial.SetFloat(OutlineEnabled, 1);
    }

    public GameObject CurrentTarget()
    {
        return currentTarget;
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            rend.sharedMaterial.SetFloat(OutlineEnabled, 0);
            return;
        }

        if (_timeBtwAttack <= 0)
        {
            Hit(currentTarget);
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
            currentTarget = null;
            rend.sharedMaterial.SetFloat(OutlineEnabled, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameObject.transform.position, _gameSettings.interactionDistance);
    }
}