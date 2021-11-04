using JetBrains.Annotations;
using Kryz.CharacterStats;
using UnityEngine;

public class RedPosture : MonoBehaviour
{
    private float _timeBtwPosture;
    private GameSettings _gameSettings;
    [SerializeField] private CharacterStat postureDistanceStat;
    private Renderer rend;

    // Can attack every <value> seconds
    public float startTimeBtwPosture = 0.3f;
    [SerializeField] private GameObject postureTarget;
    private static readonly int OutlineEnabled = Shader.PropertyToID("_OutlineEnabled");
    private WonderingDestinationSetterRandomNode _dsetter;

    private void Awake()
    {
        _dsetter = GetComponent<WonderingDestinationSetterRandomNode>();
        rend = GetComponent<Renderer>();
        rend.sharedMaterial.SetFloat(OutlineEnabled, 0);
    }

    public void SetTarget(GameObject target)
    {
        if (postureTarget != null) return;

        postureTarget = target;
        _dsetter.SetDestination(target);
        rend.sharedMaterial.SetFloat(OutlineEnabled, 1);
    }

    public GameObject GetPostureTarget()
    {
        return postureTarget;
    }

    private void Posture([NotNull] GameObject other)
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
            postureTarget = null;
            rend.sharedMaterial.SetFloat(OutlineEnabled, 0);

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
        if (postureTarget == null) return;

        if (_timeBtwPosture <= 0)
        {
            // small chance to stop posturing

            if (Random.Range(0f, 1f) < _gameSettings.postureEndChance)
            {
                Debug.Log("Had enough posturing!");
                GetComponent<Red>().RunAwayFrom(postureTarget);
                postureTarget = null;
                return;
            }

            Posture(postureTarget);
            _timeBtwPosture = startTimeBtwPosture;
        }
        else
        {
            _timeBtwPosture -= Time.deltaTime;
        }
    }
}