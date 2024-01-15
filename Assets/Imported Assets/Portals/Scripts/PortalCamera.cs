using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class PortalCamera : MonoBehaviour
{
    private readonly List<Portal> portals = new List<Portal>();

    [SerializeField]
    private Camera portalCamera;

    [SerializeField]
    private int iterations = 7;

    private readonly List<RenderTexture> tempTextures = new List<RenderTexture>();

    private Camera mainCamera;


    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        
        portals.Clear();
        tempTextures.Clear();

        foreach (Portal portal in FindObjectsByType<Portal>(FindObjectsSortMode.None)) {
            tempTextures.Add(new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32));
            portal.Renderer.material.mainTexture = tempTextures.Last();
            portals.Add(portal);
        }
    }

    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }

    void UpdateCamera(ScriptableRenderContext src, Camera cam)
    {
        for (int i = 0; i < portals.Count; i++) {
            
            if (!portals[i].Renderer.isVisible) continue;
            
            portalCamera.targetTexture = tempTextures[i];
            for (int r = iterations - 1; r >= 0; --r) {
                RenderCamera(portals[i], portals[i].OtherPortal, r, src);
            }
        }
    }

    private void RenderCamera(Portal inPortal, Portal outPortal, int iterationID, ScriptableRenderContext src)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform cameraTransform = portalCamera.transform;
        Transform tr = transform;
        cameraTransform.position = tr.position;
        cameraTransform.rotation = tr.rotation;

        for(int i = 0; i <= iterationID; ++i)
        {
            // Position the camera behind the other portal.
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        Matrix4x4 newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        UniversalRenderPipeline.RenderSingleCamera(src, portalCamera);

        //RenderPipeline.SubmitRenderRequest();
    }
}
