using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public int maxInhabitants = 20;
    public int initialInhabitants = 2;
    public float baseHealth = 50;
    public float attackDistance = 1;

    public float killHealthBonus = 50;
    public float runAwayHealthBonus = 5;
    public float hitHealthDamage = -1;
    public float reproduceAtHealth = 100;

    public int reproduceInto = 2;
    public float reproductionDelay = 4f;

    public bool allowRandomSpawn;

    public LayerMask whatIsEnemies;
    public string birdTag = "bird";
}