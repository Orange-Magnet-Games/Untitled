using System;
using UnityEngine;


[Serializable, CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 1)]
public class Gun : ScriptableObject {
  public enum AmmoType {
    SMG, AR, 
    SHOTGUN, SNIPER, 
    PISTOL, MINI_GUN, 
    GRENADE_POUCH
  }

  [Tooltip("In rounds per second"), Header("Stats")] public float  fireRate;
  public float reloadTime;
  public int damage;
  public float adsSpeed;
  public float bulletsPerShot;
  public float recoil;

  [Tooltip("In degrees of deviation from center"), Header("Accuracy")] public float bestAccuracy;
  [Tooltip("In degrees of deviation from center")] public float worstAccuracy;
  [Tooltip("In degrees per bullet")] public float inaccuracySpeed;
  // ReSharper disable once IdentifierTypo
  [Tooltip("In degrees per second")] public float reAccurracySpeed;

  [Header("Ammo")] public AmmoType ammoType;
  public int maxAmmoInMag, ammoInMag, ammoTotal;

  [Header("Visual")] public GameObject model;
  public Vector3 hipPos, adsPos;
  public Vector3 gunUIScale;
}

