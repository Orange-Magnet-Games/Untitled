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
    
    public AmmoType ammoType;
    [Tooltip("In rounds per second")] public float  fireRate;
    public float reloadTime;
    public int maxAmmoInMag, ammoInMag, ammoTotal, damage;
    public float adsSpeed;
    public float bulletsPerShot;
    
    
    [Tooltip("In degrees of deviation from center")] public float bestAccuracy, worstAccuracy;
    [Tooltip("In degrees per bullet")] public float inaccuracySpeed;
    // ReSharper disable once IdentifierTypo
    [Tooltip("In degrees per second")] public float reAccurracySpeed;
    public float recoil;
    public GameObject model;
    
    public Vector3 hipPos, adsPos;
    public Vector3 gunUIScale;
}

