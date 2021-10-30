using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject redPrefab;
    public Transform spawnPoint;

    public int currentInhabitants;

    private List<GameObject> _killedInhabitants = new();

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
        if (_inhabitants.Count < GameSettings.MaxInhabitants)
        {
            GameObject i = SpawnRed(spawnPoint, behavior);
            _inhabitants.Add(i);
        }
    }

    void Start()
    {
        foreach (Behavior behavior in Enum.GetValues(typeof(Behavior)))
        {
            _numInhabitants[behavior] = 0;
        }

        for (int i = 0; i < GameSettings.InitialInhabitants; i++)
        {
            Behavior b = i % 2 == 0 ? Behavior.Dove : Behavior.Hawk;
            SpawnInhabitant(b);
        }
        // InvokeRepeating(nameof(SpawnInhabitant), 3, 2);
    }

    private void Update()
    {
        foreach (GameObject i in _inhabitants)
        {
            Red red = i.GetComponent<Red>();
            if (red.IsDead())
            {
                _numInhabitants[red.behavior]--;
                _killedInhabitants.Add(i);
            }
        }

        foreach (GameObject i in _killedInhabitants)
        {
            _inhabitants.Remove(i);
            Destroy(i);
        }

        _killedInhabitants.Clear();

        currentInhabitants = _inhabitants.Count;
    }
}