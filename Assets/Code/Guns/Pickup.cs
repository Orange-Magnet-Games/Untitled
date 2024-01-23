using System;
using UnityEngine;

public class Pickup : MonoBehaviour {
  public int ammoInGun;
  public int gun;
  [SerializeField] private Transform gunModel;
  private GunController gunManager;

  private void Start() {
    gunManager = GameManager.instance.gunManager;
    Instantiate(gunManager.allPossibleGuns[gun].model, gunModel);
  }

  private void OnDrawGizmos() {
    Gizmos.DrawSphere(transform.position, .5f);
  }
}
