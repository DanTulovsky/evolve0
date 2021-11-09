using UnityEngine;

public class Destroy : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.StartListening("destroy", DestroyInhabitant);
    }

    private void DestroyInhabitant(GameObject gameObject)
    {
        GameManager.instance.inhabitants.Remove(gameObject);
        Destroy(gameObject);
    }
}