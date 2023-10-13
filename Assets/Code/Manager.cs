using UnityEngine;

public class Manager : MonoBehaviour {
    
    // ReSharper disable once MemberCanBePrivate.Global Will use eventually stfu
    public static Manager instance;
    private void Awake() {
        if (instance) Destroy(gameObject);
        else {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        
    }


    public PlayerController player;
    public Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
        player = GetComponentInChildren<PlayerController>();
    }
}
