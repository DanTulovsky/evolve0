using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [Header("World Settings")]
    [Tooltip("max inhabitants in the world.")]
    public int maxInhabitants = 20;
    [Tooltip("Spawn this many inhabitants initially.")]
    public int initialInhabitants = 2;
    [Tooltip("How far away for a fight/posture/etc...")]
    public float interactionDistance = 1;

    [Header("Spawn Settings")]
    [Tooltip("Interval between initial spawns")]
    public float initialSpawnDelay = 6f;
    [Tooltip("Enable random spawns")]
    public bool allowRandomSpawn;
    [Tooltip("Spawn random things every this many seconds")]
    public float randomSpawnInterval = 10;

    [Header("Health Settings")]
    [Tooltip("Base health")]
    public float baseHealth = 50;
    [Tooltip("Gain this much health if fight is won")]
    public float winHealthImpact = 50;
    [Tooltip("Lose this much health if runAwayAtHealth is reached (not used)")]
    public float injuredHealthImpact = -100;
    [Tooltip("At this health, run away from the fight")]
    public float runAwayAtHealth = 5;
    [Tooltip("Lose this much health if fight is lost")]
    public float loseHealthImpact = 0;
    [Tooltip("Lose this much health after wasting time in a non-fight")]
    public float timeWasteHealthImpact = -10;
    [Tooltip("Each hit takes this much health")]
    public float hitHealthDamage = -5;

    [Header("Reproduction Settings")]
    [Tooltip("Enables reproduction")]
    public bool enableReproduction = false;
    [Tooltip("Reprodcues when this health is reached")]
    public float reproduceAtHealth = 100;
    [Tooltip("Reproduces into this many things")]
    public int reproduceInto = 2;
    [Tooltip("Delay between spawns after reproduction")]
    public float reproductionDelay = 4f;

    [Header("Dove Settings")]
    [Tooltip("The chance that posturing will end (out of 1)")]
    public float postureEndChance = 0.1f;

    [Header("Other Settings")]
    [Tooltip("Defines the mask of what is considered enemies")]
    public LayerMask whatIsEnemies;
    [Tooltip("Unused")]
    public string birdTag = "bird";
}