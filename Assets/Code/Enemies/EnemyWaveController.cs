using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;

public class EnemyWaveController : MonoBehaviour {
    [SerializeField] private EnemyController[] enemies;

    public int[] waves;
    private readonly List<EnemyController> activeEnemies = new List<EnemyController>();

    [SerializeField] private Transform[] spawnPositions; 
    private int currentWave;
    
    [SerializeField] private float enemySpawnRate;
    private float timer;

    [SerializeField] private Animator doorAnim;
    
    private TMP_Text waveText;
    private static readonly int Closed = Animator.StringToHash("Closed");

    [HideInInspector] public bool wavesDone, canSpawn;
    public NavMeshSurface surface;
    private float navmeshTimer = 0.5f;

    private void Start() {
        waveText = GameManager.instance.uiManager.waveText;
        surface = GetComponent<NavMeshSurface>();
    }

    private void Update() {
        navmeshTimer -= navmeshTimer >= 0 ? Time.deltaTime : 0;
        if (!surface.navMeshData && navmeshTimer < 0f) {
            surface.RemoveData();
            surface.BuildNavMesh();
        }
        if (currentWave >= waves.Length) {
            waveText.text = "Waves\nComplete";
            doorAnim.SetBool(Closed, false);
            wavesDone = true;
            return;
        }
        if (!canSpawn) return;

        waveText.text = $"Wave: {currentWave+1}/{waves.Length}\nEnemies: {activeEnemies.Count}";
        if (activeEnemies.Count <= 0 && waves[currentWave] <= 0) {
            currentWave++;
            if (currentWave >= waves.Length) return;
        }

        activeEnemies.RemoveAll(x => !x);
        
        if (waves[currentWave] <= 0) return;
        
        if (timer > 0) {
            timer -= Time.deltaTime;
            return;
        }
        
        activeEnemies.Add(Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPositions[Random.Range(0, spawnPositions.Length)].position, Quaternion.identity));
        timer = enemySpawnRate;
        waves[currentWave]--;
    }
}
