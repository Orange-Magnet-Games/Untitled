using UnityEngine;

public class FaceMainCamera : MonoBehaviour { 
  private Transform cam, self; 
  [SerializeField] private Vector3 offset;
  private void Start() { 
    cam = GameManager.instance.mainCamera.transform; 
    self = transform; 
  }
  
  private void LateUpdate() { 
    self.LookAt(cam); 
    self.eulerAngles += offset; 
  }
}
