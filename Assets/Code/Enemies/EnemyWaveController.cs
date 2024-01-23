using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyWaveController : MonoBehaviour {
    [SerializeField] private EnemyController[] enemies;

    [SerializeField] private int[] waves;
    private readonly List<EnemyController> activeEnemies = new List<EnemyController>();

    [SerializeField] private Transform[] spawnPositions; 
    private int currentWave;
    
    [SerializeField] private float enemySpawnRate;
    private float timer;

    private TMP_Text waveText;

    private void Start() {
        waveText = GameManager.instance.uiManager.waveText;
    }

    private void Update() {
        if (currentWave >= waves.Length) {
            waveText.text = "Waves\nComplete";
            return;
        }

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
