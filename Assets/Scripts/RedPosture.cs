using Kryz.CharacterStats;
using UnityEngine;

public class RedPosture : MonoBehaviour
{
    private float _timeBtwPosture;
    private GameSettings _gameSettings;
    [SerializeField] private CharacterStat postureDistanceStat;

    // Can attack every <value> seconds
    public float startTimeBtwPosture = 0.3f;
    private GameObject _postureTarget;

    public void SetTarget(GameObject target)
    {
        _postureTarget = target;
    }

    private void Posture(GameObject other)
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
            _postureTarget = null;
            me.healthStat.BaseValue += _gameSettings.winHealthImpact;
            // But we wasted time
            me.healthStat.BaseValue += _gameSettings.timeWasteHealthImpact;
        }
    }

    private void Start()
    {
        _gameSettings = GameManager.Instance.gameSettings;
        postureDistanceStat.BaseValue = _gameSettings.interactionDistance;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_postureTarget == null) return;

        if (_timeBtwPosture <= 0)
        {
            Posture(_postureTarget);
            _timeBtwPosture = startTimeBtwPosture;
        }
        else
        {
            _timeBtwPosture -= Time.deltaTime;
        }
    }
}