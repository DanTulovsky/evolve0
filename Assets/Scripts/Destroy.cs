using UnityEngine;

public class Destroy : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.StartListening("destroy", DestroyInhabitant);
    }

    private static void DestroyInhabitant(GameObject gameObject)
    {
        GameManager.instance.inhabitants.Remove(gameObject);
        Destroy(gameObject);
    }
}