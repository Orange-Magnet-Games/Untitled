using System;
using UnityEngine;

public class Portal : MonoBehaviour {
  
  //----------------- WIP -------------------
  
  public Portal targetPortal;

  public Transform normalVisible;
  public Transform normalInvisible;

  public Camera portalCamera;
  public Renderer viewthroughRenderer;
  private RenderTexture viewthroughRenderTexture;
  private Material viewthroughMaterial;
  
  private Camera mainCamera;

  
  private Vector4 vectorPlane;
  
  public static Vector3 TransformPositionBetweenPortals(Portal sender, Portal target, Vector3 position) => 
    target.normalInvisible.TransformPoint(sender.normalVisible.InverseTransformPoint(position));
  

  public static Quaternion TransformRotationBetweenPortals(Portal sender, Portal target, Quaternion rotation) =>
    target.normalInvisible.rotation * Quaternion.Inverse(sender.normalVisible.rotation) * rotation;
  

  private void Start() { // Create Render Texture

    viewthroughRenderTexture = new RenderTexture(
      Screen.width, Screen.height,
      24,
      RenderTextureFormat.DefaultHDR);
    
    viewthroughRenderTexture.Create();

    viewthroughMaterial = viewthroughRenderer.material;
    viewthroughMaterial.mainTexture = viewthroughRenderTexture;

    portalCamera.targetTexture = viewthroughRenderTexture;

    mainCamera = GameManager.instance.mainCamera;
    

    Plane plane = new Plane(normalVisible.forward, transform.position);
    
    vectorPlane = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);

  }

  private void LateUpdate() {
    
    Vector3 virtualPosition = TransformPositionBetweenPortals(this, targetPortal, mainCamera.transform.position);
    
    Quaternion virtualRotation = TransformRotationBetweenPortals(this, targetPortal, mainCamera.transform.rotation);
    
    portalCamera.transform.SetPositionAndRotation(virtualPosition, virtualRotation);

    Vector4 clipThroughSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * targetPortal.vectorPlane;

    Matrix4x4 obliqueProjectionMatrix = mainCamera.CalculateObliqueMatrix(clipThroughSpace);
    portalCamera.projectionMatrix = obliqueProjectionMatrix;
    
    portalCamera.Render();
  }

  private void OnDestroy() {
    
    viewthroughRenderTexture.Release();
    
    Destroy(viewthroughMaterial);
    Destroy(viewthroughRenderTexture);
  }
}