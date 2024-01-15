using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [field: SerializeField]
    public Portal OtherPortal { get; private set; }

    private readonly List<PortalableObject> portalObjects = new List<PortalableObject>();
    
    public Collider wallCollider;

    // Components.
    public Renderer Renderer { get; private set; }

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
    }

    private void Start() {
        // ReSharper disable once Unity.InefficientPropertyAccess
        transform.position -= transform.forward * 0.005f;
        //gameObject.SetActive(false);
    }

    private void Update() {
        Renderer.enabled = true;

        foreach (PortalableObject t in from t in portalObjects let objPos = transform.InverseTransformPoint(t.transform.position) where objPos.z > 0.0f select t) {
            t.Warp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PortalableObject obj = other.GetComponent<PortalableObject>();
        
        if (obj == null) return;
        
        portalObjects.Add(obj);
        obj.SetIsInPortal(this, OtherPortal, wallCollider);
        obj.SetIsInPortal(this, OtherPortal, OtherPortal.wallCollider);
    }

    private void OnTriggerExit(Collider other)
    {
        PortalableObject obj = other.GetComponent<PortalableObject>();

        if (!portalObjects.Contains(obj)) return;
        
        portalObjects.Remove(obj);
        obj.ExitPortal(OtherPortal.wallCollider);
        obj.ExitPortal(wallCollider);
    }
}
