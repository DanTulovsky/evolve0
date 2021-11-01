using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public int maxInhabitants = 20;
    public int initialInhabitants = 2;
    public float baseHealth = 50;
    public float interactionDistance = 1;
    // The chance that posturing will end (out of 1)
    public float postureEndChance = 0.1f;

    // Gain this much health if fight is won
    public float winHealthImpact = 50;
    // Lose this much health if runAwayAtHealth is reached
    public float injuredHealthImpact = -100;
    // At this health, run away from the fight
    public float runAwayAtHealth = 5;
    // Lose this much health if fight is lost
    public float loseHealthImpact = 0;
    // Lose this much health after wasting time in a non-fight
    public float timeWasteHealthImpact = -10;
    
    public float hitHealthDamage = -1;
    public float reproduceAtHealth = 100;

    public int reproduceInto = 2;
    public float reproductionDelay = 4f;

    public bool allowRandomSpawn;

    public LayerMask whatIsEnemies;
    public string birdTag = "bird";
}