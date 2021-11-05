using UnityEngine;
using UnityEngine.UI;

public class TriggerButton : MonoBehaviour
{
    public Button triggerButton;
    // Start is called before the first frame update
    private void Start()
    {
        triggerButton.onClick.AddListener(SendEvent);
        
    }

    // Update is called once per frame
    private void OnDisable()
    {
        //Un-Register Button Events
        triggerButton.onClick.RemoveAllListeners();
    }

    private void SendEvent()
    {
       EventManager.TriggerEvent("buttonPressed");
       EventManager.TriggerEvent("buttonPressed", "hello there!");
    }
}
