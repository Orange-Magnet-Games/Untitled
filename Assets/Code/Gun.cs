using System;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 1)]
public class Gun : ScriptableObject {
    public enum GunType {
        SMG, AR, SHOTGUN, SNIPER, PISTOL, MINIGUN, GRENADE_POUCH
    }
  
    public GunType type;
    [Tooltip("In rounds per second")] public float  fireRate;
    public float reloadTime;
    public int maxAmmoInMag, ammoInMag, ammoTotal, damage;

  
}