using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Tooltip("Prefab for the inhabitants")]
    public GameSettings gameSettings;

    public TMP_Text hawksTextField;
    public TMP_Text dovesTextField;

    private readonly List<GameObject> killedInhabitants = new();
    private readonly List<GameObject> reproduceInhabitants = new();

    public readonly List<GameObject> inhabitants = new();

    public readonly Dictionary<Behavior, Color> behaviorToColor = new()
    {
        [Behavior.Dove] = Color.green,
        [Behavior.Hawk] = Color.red
    };

    public enum Behavior
    {
        Hawk,
        Dove
    };

    private void Start()
    {
        gameSettings = GetComponent<GameSettings>();

        // Add any manually placed Birds to the list
        // For debug only
        var birds = FindObjectsOfType<Bird>();

        foreach (Bird bird in birds)
        {
            bird.GetComponent<SpriteRenderer>().color = behaviorToColor[bird.behavior];
            inhabitants.Add(bird.gameObject);
            FindObjectOfType<Spawn>().SetDefaultStats(bird.gameObject, bird.behavior);
        }
    }

    private void Update()
    {
        foreach (GameObject i in inhabitants)
        {
            Bird bird = i.GetComponent<Bird>();
            bird.GetComponent<SpriteRenderer>().color = behaviorToColor[bird.behavior];

            if (bird.IsDead())
            {
                killedInhabitants.Add(i);
            }

            if (bird.CanReproduce())
            {
                reproduceInhabitants.Add(i);
            }
        }

        foreach (GameObject i in killedInhabitants)
        {
            inhabitants.Remove(i);
            Destroy(i);
        }

        killedInhabitants.Clear();

        foreach (GameObject i in reproduceInhabitants)
        {
            Bird bird = i.GetComponent<Bird>();
            // Destroy the old
            EventManager.TriggerEvent("destroy", bird.gameObject);

            // Spawn two new ones of the same behavior
            EventManager.TriggerEvent("spawnWithDelay", bird.behavior);
        }

        reproduceInhabitants.Clear();

        hawksTextField.text = $"hawks: {CountOf(Behavior.Hawk).ToString()}";
        dovesTextField.text = $"doves: {CountOf(Behavior.Dove).ToString()}";
    }

    private int CountOf(Behavior behavior)
    {
        return inhabitants.Count(inhabitant => inhabitant.GetComponent<Bird>().behavior == behavior);
    }
}