using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {
    private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private Slider healthSlider;
    private void Start() {
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
    }

    public void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) Destroy(gameObject);
    }

    private void Update() {
        healthSlider.value = health;
    }
}
