using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class GameManager : Singleton<GameManager>
{
    [Tooltip("Prefab for the inhabitants")]
    public GameObject redPrefab;
    public Transform spawnPoint;
    public GameSettings gameSettings;

    public TMP_Text hawksTextField;
    public TMP_Text dovesTextField;

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

    public readonly List<GameObject> inhabitants = new();
    public readonly Dictionary<Behavior, int> numInhabitants = new();


    private GameObject SpawnRed(Transform parent, Behavior behavior)
    {
        GameObject i = Instantiate(redPrefab, parent.position, parent.rotation, parent);

        Red red = i.GetComponent<Red>();
        red.behavior = behavior;
        red.GetComponent<SpriteRenderer>().color = _behaviorToColor[behavior];
        i.name = $"{red.behavior.ToString()}";

        numInhabitants[behavior]++;

        return i;
    }

    private void SpawnInhabitant(Behavior behavior)
    {
        if (inhabitants.Count < gameSettings.maxInhabitants)
        {
            GameObject i = SpawnRed(spawnPoint, behavior);
            inhabitants.Add(i);
        }
        else
        {
            Debug.LogFormat("Unable to spawn, max inhabitants ({0} reached.", gameSettings.maxInhabitants);
        }
    }

    private void SpawnRandomInhabitant()
    {
        if (_initialSpawn) return;
        if (!gameSettings.allowRandomSpawn) return;

        Array values = Enum.GetValues(typeof(Behavior));
        Random random = new();
        Behavior randomBehavior = (Behavior)values.GetValue(random.Next(values.Length));

        if (inhabitants.Count >= gameSettings.maxInhabitants) return;

        SpawnInhabitant(randomBehavior);
    }

    private IEnumerator SpawnRandomInhabitantWithDelay(float delay)
    {
        for (int i = 0; i < gameSettings.initialInhabitants; i++)
        {
            Behavior behavior = i % 2 == 0 ? Behavior.Dove : Behavior.Hawk;

            SpawnInhabitant(behavior);
            yield return new WaitForSeconds(delay);
        }

        Debug.Log("done with initial spawn");
        _initialSpawn = false;
    }

    private void Start()
    {
        gameSettings = GetComponent<GameSettings>();
        var behaviors = Enum.GetValues(typeof(Behavior));

        foreach (Behavior behavior in behaviors)
        {
            numInhabitants[behavior] = 0;
        }

        _initialSpawn = true;
        // TODO: Move these to Spawn script
        StartCoroutine(SpawnRandomInhabitantWithDelay(gameSettings.initialSpawnDelay));
        InvokeRepeating(nameof(SpawnRandomInhabitant), 3, gameSettings.randomSpawnInterval);
    }

    private void Update()
    {
        foreach (GameObject i in inhabitants)
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
            numInhabitants[i.GetComponent<Red>().behavior]--;
            inhabitants.Remove(i);
            Destroy(i);
        }

        _killedInhabitants.Clear();

        foreach (GameObject i in _reproduceInhabitants)
        {
            Behavior behavior = i.GetComponent<Red>().behavior;
            Debug.LogFormat("[{0} reproducing", behavior);

            // Destroy the old
            inhabitants.Remove(i);
            // TODO: This should be handled by a Destroy script via an event
            Destroy(i);

            // Spawn two new ones of the same behavior
            Debug.LogFormat("spawning after reproduction: {0}", behavior);
            EventManager.TriggerEvent("spawnWithDelay", behavior);
        }

        _reproduceInhabitants.Clear();

        currentInhabitants = inhabitants.Count;

        hawksTextField.text = $"hawks: {numInhabitants[Behavior.Hawk].ToString()}";
        dovesTextField.text = $"doves: {numInhabitants[Behavior.Dove].ToString()}";
    }
}