using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

public class Spawn : MonoBehaviour
{
    public GameObject birdPrefab;
    public Transform spawnPoint;

    private GameSettings _gameSettings;
    private GameManager gameManager;
    private bool _initialSpawn;


    private void OnEnable()
    {
        EventManager.StartListening("spawn", SpawnInhabitant);
        EventManager.StartListening("spawnWithDelay", SpawnInhabitantWithDelay);
    }

    private void Awake()
    {
        gameManager = GameManager.instance;
        _gameSettings = gameManager.gameSettings;
    }

    private void Start()
    {
        _initialSpawn = true;

        StartCoroutine(SpawnRandomInhabitantWithDelay(_gameSettings.spawnDelay));
        InvokeRepeating(nameof(SpawnRandomInhabitant), 3, _gameSettings.randomSpawnInterval);
    }

    private void SpawnRandomInhabitant()
    {
        if (_initialSpawn) return;
        if (!_gameSettings.allowRandomSpawn) return;

        Array values = Enum.GetValues(typeof(GameManager.Behavior));
        Random random = new();
        GameManager.Behavior randomBehavior = (GameManager.Behavior)values.GetValue(random.Next(values.Length));

        if (GameManager.instance.inhabitants.Count >= _gameSettings.maxInhabitants) return;

        SpawnInhabitant(randomBehavior);
    }

    private IEnumerator SpawnRandomInhabitantWithDelay(float delay)
    {
        for (int i = 0; i < _gameSettings.initialInhabitants; i++)
        {
            GameManager.Behavior behavior = i % 2 == 0 ? GameManager.Behavior.Dove : GameManager.Behavior.Hawk;

            SpawnInhabitant(behavior);
            yield return new WaitForSeconds(delay);
        }

        _initialSpawn = false;
    }

    public void SpawnDove()
    {
        SpawnInhabitant(GameManager.Behavior.Dove);
    }
    public void SpawnHawk()
    {
        SpawnInhabitant(GameManager.Behavior.Hawk);
    }

    private void SpawnInhabitant(GameManager.Behavior behavior)
    {
        if (gameManager.inhabitants.Count < _gameSettings.maxInhabitants)
        {
            GameObject i = SpawnBird(spawnPoint, behavior);
            gameManager.inhabitants.Add(i);
        }
        else
        {
            Debug.LogFormat("Unable to spawn, max inhabitants ({0} reached.", _gameSettings.maxInhabitants);
        }
    }

    private void SpawnInhabitantWithDelay(GameManager.Behavior behavior)
    {
        StartCoroutine(SpawnInhabitantWithDelay(behavior, 6f));
    }

    private IEnumerator SpawnInhabitantWithDelay(GameManager.Behavior behavior, float delay)
    {
        for (int i = 0; i < _gameSettings.reproduceInto; i++)
        {
            yield return new WaitForSeconds(delay);
            SpawnInhabitant(behavior);
        }
    }

    private GameObject SpawnBird(Transform parent, GameManager.Behavior behavior)
    {
        GameObject i = Instantiate(birdPrefab, parent.position, parent.rotation, parent);
        SetDefaultStats(i, behavior);

        return i;
    }

    public void SetDefaultStats(GameObject go, GameManager.Behavior behavior)
    {
        Bird bird = go.GetComponent<Bird>();
        bird.behavior = behavior;
        bird.GetComponent<SpriteRenderer>().color = GameManager.instance.behaviorToColor[behavior];
        go.name = $"{bird.behavior.ToString()}";

        switch (bird.behavior)
        {
            case GameManager.Behavior.Hawk:
                bird.speedStat.BaseValue = _gameSettings.hawkSpeed;
                break;
            case GameManager.Behavior.Dove:
                bird.speedStat.BaseValue = _gameSettings.doveSpeed;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(behavior), behavior, null);
        }
    }
}
