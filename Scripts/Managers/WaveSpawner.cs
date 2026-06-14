using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour {
    public static WaveSpawner instance { get; private set; } // Singleton

    public List<GameObject> EnemiesPrefabsList = new List<GameObject>(); // -> scriptable Object // Should be in a config ScriptableObject

    [SerializeField] private List<Waypoints> waypointsList = new List<Waypoints>(); // One "Waypoints" per Path 

    [HideInInspector] public List<Enemy> EnemiesList = new List<Enemy>(); // All Enemies on the board 

    [SerializeField] private Transform enemyPrefab;
    //[SerializeField] private Transform spawnPoint; // List ? -> if 2 spawn points

    [SerializeField] private float timeBetweenWaves = 4f;
    [SerializeField] private float timeBetweenSpawn = 1f;

    //private float countdown = 2f;  
    //[SerializeField] private int waveIndex = 1;
    private int spawnIndex = 0;
    private WaitForSeconds waitBetweenWaves;
    private WaitForSeconds waitBetweenSpawn;

    [SerializeField] private int currentWave = 1;

    private static readonly int[,] waves =  // => Scriptable Object 
       {{  2,  0,  0,  0,  0 },  // Wave 1
        {  4,  0,  0,  0,  0 },  // Wave 2
        {  6,  1,  0,  0,  0 },  // Wave 3
        {  2,  4,  0,  0,  0 },  // Wave 4
        {  0,  6,  1,  0,  0 },  // Wave 5
        {  0,  2,  1,  0,  0 },  // Wave 6
        {  0,  1,  2,  1,  0 },  // Wave 7
        {  0,  0,  4,  2,  1 },  // Wave 8
        {  0,  0,  6,  2,  4 },  // Wave 9
        {  0,  0,  1,  2,  6 }}; // Wave 10

    //Pow   1   2   3   4   5

    private static readonly int[,] wavesTest =
       {{  2,  0,  0,  0,  0 },  // Wave 1
        {  4,  2,  1,  1,  1 }};  // Wave 2

    private void Awake() {
        if (instance != null) Debug.LogWarning("More than one WaveSpawner detected");
        instance = this;
    }

    private void Start() {
        waitBetweenWaves = new(timeBetweenWaves);
        waitBetweenSpawn = new(timeBetweenSpawn);

        StartCoroutine(SpawnAllWaves(waves));
    }

    private void SpawnOneEnemyFromPower(int power) {
        Waypoints chosenPath = GetRandomPath();
        Transform firstPoint = chosenPath.points[0].transform;

        Transform myEnemy = Instantiate(EnemiesPrefabsList[power].transform, firstPoint.position, firstPoint.rotation);
        myEnemy.gameObject.name = "Enemy " + spawnIndex;


        myEnemy.GetComponent<Enemy>().SetReferences(this, chosenPath); // each enemy choose a random path 

        spawnIndex++;
        //EnemiesList.Add(enemyPrefab.GetComponent<Enemy>()); // -> In Enemy.cs
    }

    private Waypoints GetRandomPath() {
        int randomIndex = Random.Range(0, waypointsList.Count); // inclusiv / exclusiv 
        return waypointsList[randomIndex];
    }

    private void WinTheLevel() {
        int currentLevelIndex = int.Parse(SceneManager.GetActiveScene().name.Replace("Lvl_", "")) - 1; // subtract 1 to make it 0-based

        SaveManager.Instance.UnlockNextLevel(currentLevelIndex, 1); // currentLevelIndex // starsEarned

        UIManager.Instance.ShowScreen(ScreenType.Victory); 
    }

    private IEnumerator SpawnAllWaves(int[,] waves) {

        int waveNumber = waves.GetLength(0);
        int poolNumber = waves.GetLength(1);


        for (int waveIndex = 0; waveIndex < waveNumber; waveIndex++) {

            for (int poolIndex = 0; poolIndex < poolNumber; poolIndex++) {

                int poolSize = waves[waveIndex, poolIndex];

                for (int k = 0; k < poolSize; k++) {
                    yield return waitBetweenSpawn;
                    SpawnOneEnemyFromPower(poolIndex);
                }
                // Time Between Pool
                yield return waitBetweenSpawn; // New
            }
            yield return waitBetweenWaves;
            currentWave++;
        }

        while (true) {
            yield return waitBetweenSpawn;
            if (EnemiesList.Count == 0) {
                // END OF THE LEVEL
                // All Enemies are dead
                WinTheLevel(); break;
            }
        }
    }
}
