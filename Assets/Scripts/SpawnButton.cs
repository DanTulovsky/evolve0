using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    private TMP_Dropdown birdDropdown;

    public Button spawnButton;

    // Start is called before the first frame update
    private void Start()
    {
        birdDropdown = GameObject.Find("birdDropdown").GetComponent<TMP_Dropdown>();
        birdDropdown.ClearOptions();
        birdDropdown.AddOptions(Enum.GetNames(typeof(GameManager.Behavior)).ToList());

        spawnButton.onClick.AddListener(SendSpawnEvent);
    }

    private void OnDisable()
    {
        spawnButton.onClick.RemoveAllListeners();
    }

    private void SendSpawnEvent()
    {
        GameManager.Behavior behavior;

        switch (birdDropdown.options[birdDropdown.value].text)
        {
            case "Hawk":
                behavior = GameManager.Behavior.Hawk;
                break;
            case "Dove":
                behavior = GameManager.Behavior.Dove;
                break;
            default:
                throw new NotImplementedException();
        }

        EventManager.TriggerEvent("spawn", behavior);
    }
}