using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameManager : Singleton<GameManager>
{
    public GameObject redPrefab;
    public Transform spawnPoint;
    public GameSettings gameSettings;

    public int currentInhabitants;

    private readonly List<GameObject> _killedInhabitants = new();
    private readonly List<GameObject> _reproduceInhabitants = new();
    private bool _initialSpawn;

    public enum Behavior
    {
        Hawk,
        Dove
    };

    private readonly Dictionary<Behavior, Color> _behaviorToColor = new Dictionary<Behavior, Color>
    {
        [Behavior.Dove] = Color.green,
        [Behavior.Hawk] = Color.red
    };

    private readonly List<GameObject> _inhabitants = new();
    private readonly Dictionary<Behavior, int> _numInhabitants = new();


    private GameObject SpawnRed(Transform parent, Behavior behavior)
    {
        GameObject i = Instantiate(redPrefab, parent.position, parent.rotation, parent);

        Red red = i.GetComponent<Red>();
        red.behavior = behavior;
        red.GetComponent<SpriteRenderer>().color = _behaviorToColor[behavior];

        _numInhabitants[behavior]++;

        return i;
    }

    private void SpawnInhabitant(Behavior behavior)
    {
        if (_inhabitants.Count < gameSettings.maxInhabitants)
        {
            GameObject i = SpawnRed(spawnPoint, behavior);
            _inhabitants.Add(i);
        }
        else
        {
            Debug.LogFormat("Unable to reproduce, max inhabitants ({0} reached.", gameSettings.maxInhabitants);
        }
    }

    private void SpawnRandomInhabitant()
    {
        if (_initialSpawn) return;
        if (!gameSettings.allowRandomSpawn) return;

        Array values = Enum.GetValues(typeof(Behavior));
        Random random = new Random();
        Behavior randomBehavior = (Behavior)values.GetValue(random.Next(values.Length));

        if (_inhabitants.Count < gameSettings.maxInhabitants)
        {
            GameObject i = SpawnRed(spawnPoint, randomBehavior);
            _inhabitants.Add(i);
        }
    }

    private IEnumerator SpawnRandomInhabitantWithDelay(float delay)
    {
        for (int i = 0; i < gameSettings.initialInhabitants; i++)
        {
            Behavior behavior = i % 2 == 0 ? Behavior.Dove : Behavior.Hawk;

            SpawnInhabitant(behavior);
            yield return new WaitForSeconds(delay);
        }

        _initialSpawn = false;
    }

    private IEnumerator SpawnInhabitantWithDelay(int number, Behavior behavior, float delay)
    {
        for (int i = 0; i < number; i++)
        {
            SpawnInhabitant(behavior);
            yield return new WaitForSeconds(delay);
        }
    }

    private void Start()
    {
        gameSettings = GetComponent<GameSettings>();

        foreach (Behavior behavior in Enum.GetValues(typeof(Behavior)))
        {
            _numInhabitants[behavior] = 0;
        }

        _initialSpawn = true;
        StartCoroutine(SpawnRandomInhabitantWithDelay(6f));

        InvokeRepeating(nameof(SpawnRandomInhabitant), 3, 10);
    }

    private void Update()
    {
        foreach (GameObject i in _inhabitants)
        {
            Red red = i.GetComponent<Red>();
            if (red.IsDead())
            {
                _killedInhabitants.Add(i);
            }

            if (red.CanReproduce())
            {
                _reproduceInhabitants.Add(i);
            }
        }

        foreach (GameObject i in _killedInhabitants)
        {
            Debug.LogFormat("Killing {0}", i.GetComponent<Red>().behavior);
            _numInhabitants[i.GetComponent<Red>().behavior]--;
            _inhabitants.Remove(i);
            Destroy(i);
        }

        _killedInhabitants.Clear();

        foreach (GameObject i in _reproduceInhabitants)
        {
            Behavior behavior = i.GetComponent<Red>().behavior;
            Debug.LogFormat("[{0} reproducing", behavior);

            // Destroy the old
            _inhabitants.Remove(i);
            Destroy(i);

            // Spawn two new ones of the same behavior
            Debug.LogFormat("spawning after reproduction: {0}", behavior);
            StartCoroutine(SpawnInhabitantWithDelay(gameSettings.reproduceInto, behavior,
                gameSettings.reproductionDelay));
        }

        _reproduceInhabitants.Clear();

        currentInhabitants = _inhabitants.Count;
    }
}