using UnityEngine;

public class Follow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool rotate = false;
    private void LateUpdate() {
        transform.position = target.position + offset;
        if (rotate) transform.rotation = target.rotation;
    }
}
