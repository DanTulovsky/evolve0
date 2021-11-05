using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
   private static T _instance;

   public static T instance
   {
      get
      {
         if (_instance != null) return _instance;
         
         _instance = FindObjectOfType<T>();
         if (_instance != null) return _instance;
         
         GameObject newGO = new GameObject();
         _instance = newGO.AddComponent<T>();

         return _instance;
      }
   }

   protected void Awake()
   {
      _instance = this as T;
   }
}