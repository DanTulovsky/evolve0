using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Spawn : MonoBehaviour
{
    public GameObject redPrefab;
    public Transform spawnPoint;

    private GameSettings _gameSettings;
    private GameManager gameManager;
    private bool _initialSpawn;

    private readonly Dictionary<GameManager.Behavior, Color> _behaviorToColor = new Dictionary<GameManager.Behavior, Color>
    {
        [GameManager.Behavior.Dove] = Color.green,
        [GameManager.Behavior.Hawk] = Color.red
    };

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

        StartCoroutine(SpawnRandomInhabitantWithDelay(_gameSettings.initialSpawnDelay));
        InvokeRepeating(nameof(SpawnRandomInhabitant), 3, _gameSettings.randomSpawnInterval);
    }

    private void Update()
    {
        foreach (GameObject i in GameManager.instance.inhabitants)
        {
            Red red = i.GetComponent<Red>();
            if (red.IsDead())
            {
                GameManager.instance.killedInhabitants.Add(i);
            }

            if (red.CanReproduce())
            {
                GameManager.instance.reproduceInhabitants.Add(i);
            }
        }

        foreach (GameObject i in GameManager.instance.reproduceInhabitants)
        {
            // Destroy the old
            EventManager.TriggerEvent("destroy");

            // Spawn two new ones of the same behavior
            EventManager.TriggerEvent("spawnWithDelay", i.GetComponent<Red>().behavior);
        }

        GameManager.instance.reproduceInhabitants.Clear();
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

        Debug.Log("done with initial spawn");
        _initialSpawn = false;
    }

    public void SpawnDove()
    {
        Debug.Log("Spawning dove.");
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
            GameObject i = SpawnRed(spawnPoint, behavior);
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
            SpawnInhabitant(behavior);
            yield return new WaitForSeconds(delay);
        }
    }

    private GameObject SpawnRed(Transform parent, GameManager.Behavior behavior)
    {
        GameObject i = Instantiate(redPrefab, parent.position, parent.rotation, parent);

        Red red = i.GetComponent<Red>();
        red.behavior = behavior;
        red.GetComponent<SpriteRenderer>().color = _behaviorToColor[behavior];
        i.name = $"{red.behavior.ToString()}";

        switch (behavior)
        {
            case GameManager.Behavior.Hawk:
                red.speedStat.BaseValue = _gameSettings.hawkSpeed;
                break;
            case GameManager.Behavior.Dove:
                red.speedStat.BaseValue = _gameSettings.doveSpeed;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(behavior), behavior, null);
        }

        return i;
    }
}
