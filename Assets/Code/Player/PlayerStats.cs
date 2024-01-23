using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour {
  private int health;
  [SerializeField] private int maxHealth;
  private float healthTimer;
  [SerializeField] private float healingDelay;
  private MeshRenderer heart;
  private Material heartMat;
  private Volume postProcess;
  private Vignette vignette;
  private static readonly int BaseColor = Shader.PropertyToID("_Base_Color");

  private void Start() {
    health = maxHealth;
    heart = GameManager.instance.uiManager.heart.GetComponent<MeshRenderer>();
    heart.material = heartMat = new Material(heart.material);
    postProcess = GameManager.instance.postProcess;
    postProcess.profile.TryGet(out vignette);
  }

  public void TakeDamage(int damage) {
    health -= damage;
    if (health <= 0) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      health = maxHealth;
    }
      
    healthTimer = healingDelay;
  }

  private void Update() {
    healthTimer -= healthTimer >= 0 ? Time.deltaTime : 0;

    vignette.intensity.value = 1 - (float)health / maxHealth;
    heartMat.SetColor(BaseColor, new Color((float)health / maxHealth, 0, 0));
    
    if (healthTimer >= 0 || health >= maxHealth) return;
        
    health++;
    healthTimer = 0.25f;
  }
}
