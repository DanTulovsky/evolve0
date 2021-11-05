using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject redPrefab;
    public Transform spawnPoint;

    private GameSettings _gameSettings;
    private GameManager gameManager;

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

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
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

        gameManager.numInhabitants[behavior]++;

        return i;
    }
}
