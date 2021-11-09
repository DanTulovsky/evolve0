using System;
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

    public readonly List<GameObject> killedInhabitants = new();
    public readonly List<GameObject> reproduceInhabitants = new();

    public enum Behavior
    {
        Hawk,
        Dove
    };

    public readonly List<GameObject> inhabitants = new();

    private void Start()
    {
        gameSettings = GetComponent<GameSettings>();
    }

    private void Update()
    {

        foreach (GameObject i in killedInhabitants)
        {
            inhabitants.Remove(i);
            Destroy(i);
        }

        killedInhabitants.Clear();

        hawksTextField.text = $"hawks: {CountOf(Behavior.Hawk).ToString()}";
        dovesTextField.text = $"doves: {CountOf(Behavior.Dove).ToString()}";
    }

    private int CountOf(Behavior behavior)
    {
        return inhabitants.Count(inhabitant => inhabitant.GetComponent<Red>().behavior == behavior);
    }
}