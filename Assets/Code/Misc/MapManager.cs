using UnityEngine;

public class MapManager : MonoBehaviour {
  [SerializeField] private MapReporter[] allMaps;
  private MapReporter currentMap, nextMap;

  [SerializeField] private int enemyAcceleration, waveAcceleration;
  private int enemies = 3, waves = 2;

  private int side = 1;

  private int roomCount;
  private UIManager ui;
  
  private void Start() {
    currentMap = CreateMap();
    nextMap = CreateMap();
    currentMap.enemyWaves.canSpawn = true;
    nextMap.enemyWaves.canSpawn = false;
    ui = GameManager.instance.uiManager;
    ui.roomCountText.text = $"Rooms Cleared: {roomCount}";
  }

  private void Update() {
    if (!nextMap.playerWalkedIn || !currentMap.enemyWaves.wavesDone) return;
    Destroy(currentMap.gameObject);
    currentMap = nextMap;
    roomCount++;
    currentMap.enemyWaves.canSpawn = true;
    nextMap = CreateMap();
    nextMap.enemyWaves.canSpawn = false;
    ui.roomCountText.text = $"Rooms Cleared: {roomCount}";
  }

  private MapReporter CreateMap() {
    MapReporter map = Instantiate(allMaps[Random.Range(0, allMaps.Length)], transform);
    map.transform.localScale = new Vector3(side, 1, 1);
    side = side < 0 ? 1 : -1;
    map.enemyWaves.waves = new int[waves];
    for (int i = 0; i < map.enemyWaves.waves.Length; i++) {
      map.enemyWaves.waves[i] = enemies;
      enemies += enemyAcceleration;
    }
    waves += waveAcceleration;
    map.enemyWaves.wavesDone = false;
    return map;
  }
}
