using System;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunController : MonoBehaviour {
  private PlayerController player;
  private Transform camTransform;
  private Camera mainCam, portalCam;
  [SerializeField] private Camera gunCam;
  //private CameraLook mainCamLook;
  private InputMaster input;
  
  [HideInInspector] public Gun[] guns = new Gun[2];
  public Gun[] allPossibleGuns;
  
  [HideInInspector] public int activeGun;
  private GunModel activeGunModel;
  [SerializeField] private MuzzleFlash muzzleFlash;
  public BulletHole bulletHole;
  [SerializeField] private ParticleSystem particles;
  private GunUI uiGuns;

  [SerializeField] private float pickupRange;

  [Serializable]
  private struct ClipGun {
    public Gun.AmmoType gun;
    public AudioClip clip;
  }

  [SerializeField] private ClipGun[] gunSounds;

  [SerializeField] private AudioClip[] reloadSounds;
  private SoundManager soundManager;
  
  private float accuracy;
  private bool shooting, aimingDownSights;
  private float shootTimer, reloadTimer, fov;
  private Quaternion shootRotation = Quaternion.identity, originalRotation = Quaternion.identity;
  [HideInInspector] public Vector3 occlusionOffset = Vector3.zero;
  
  private TMP_Text ammoText, ammoLeftText;
  [SerializeField] private Transform gunTransform;
  private Crosshair crosshair;

  [SerializeField] private int[] startingGuns = new int[2];
  private PauseMenu pause;
  private void Start() {
    pause = GameManager.instance.pauseMenu;
    soundManager = GameManager.instance.soundManager;
    
    for (int i = 0; i < 2; i++) if (startingGuns[i] > -1) { guns[i] = Instantiate(allPossibleGuns[startingGuns[i]]); activeGun = i; }
    
    activeGunModel = Instantiate(guns[activeGun].model, gunTransform).GetComponent<GunModel>();
    
    player = GameManager.instance.player;
    mainCam = GameManager.instance.mainCamera;
    portalCam = GameManager.instance.portalCamera;
    camTransform = mainCam.transform;
    //mainCamLook = mainCam.GetComponent<CameraLook>();
    input = player.input;

    input.Shooter.Shoot.performed += _ => shooting = true;
    input.Shooter.Shoot.canceled += _ => shooting = false;

    input.Shooter.Reload.performed += _ => OnReload();
    
    input.Shooter.ADS.performed += _ => aimingDownSights = true;
    input.Shooter.ADS.canceled += _ => aimingDownSights = false;

    input.Shooter.PickUp.performed += _ => GetWeapon();

    input.Shooter.WeaponScroll.performed += _ => SwitchWeapon();
    
    originalRotation = transform.localRotation;

    ammoText = GameManager.instance.uiManager.ammoText;
    ammoLeftText = GameManager.instance.uiManager.ammoLeftText;
    crosshair = GameManager.instance.uiManager.crosshair;
    uiGuns = GameManager.instance.uiManager.guns;
    
    UpdateUIWeapons();
    
    uiGuns.gunIndex = activeGun;

  }

  private void GetWeapon() {
    Debug.DrawRay(camTransform.position, camTransform.forward * pickupRange, Color.red, 10);
    if (!Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hit, pickupRange, LayerMask.GetMask("Collectible"))) return;
    if (!hit.transform.TryGetComponent(out Pickup pickup)) return;
    
    if (!guns[1]) {
      guns[1] = Instantiate(allPossibleGuns[pickup.gun]);
      guns[1].ammoTotal = pickup.ammoInGun - guns[1].maxAmmoInMag;
      guns[1].ammoInMag = guns[1].maxAmmoInMag;
    }
    else {
      guns[activeGun] = Instantiate(allPossibleGuns[pickup.gun]);
      guns[activeGun].ammoTotal = pickup.ammoInGun - guns[activeGun].maxAmmoInMag;
      guns[activeGun].ammoInMag = guns[activeGun].maxAmmoInMag;
    }
    Destroy(pickup.gameObject);
    SwitchWeapon();
    SwitchWeapon();
    UpdateUIWeapons();
  }
  private void SwitchWeapon() {
    
    
    if (reloadTimer >= 0) {
      reloadTimer = 0;
      guns[activeGun].ammoInMag = 0;
    }
    switch (activeGun) {
      case 1 when !guns[0]: return;
      case 1: activeGun = 0; break;
      case 0 when !guns[1]: return;
      case 0: activeGun = 1; break;
    }
    
    uiGuns.gunIndex = activeGun;
    
    Destroy(activeGunModel.gameObject);
    activeGunModel = Instantiate(guns[activeGun].model, gunTransform).GetComponent<GunModel>();
    transform.localRotation = originalRotation;
    gunTransform.localRotation = Quaternion.Euler(0, 0, 0);
    gunTransform.localPosition = guns[activeGun].hipPos + occlusionOffset;
    aimingDownSights = false;
    
  }

  private void UpdateUIWeapons() {

    for (int i = 0; i < guns.Length; i++) {
      if (!guns[i]) continue;
      foreach(Transform child in uiGuns.gunPivots[i].transform) Destroy(child.gameObject);

      GameObject gj = Instantiate(guns[i].model, uiGuns.gunPivots[i]);
      gj.transform.localScale = guns[i].gunUIScale;
    }
  }
  
  private void Update() {
    if (pause.paused) return;
    
    shootTimer -= shootTimer >= 0 ? Time.deltaTime : 0;
    reloadTimer -= reloadTimer >= 0 ? Time.deltaTime : 0;
    accuracy = Mathf.Clamp(accuracy - (accuracy >= guns[activeGun].bestAccuracy ? Time.deltaTime * guns[activeGun].reAccurracySpeed : 0), guns[activeGun].bestAccuracy, aimingDownSights ? guns[activeGun].worstAccuracy / 2 : guns[activeGun].worstAccuracy);
    crosshair.LineMovement = accuracy;

    
    // ReSharper disable twice Unity.PerformanceCriticalCodeInvocation
    if (shooting) Shoot();
    AimDownSights();
    
    PositionGun();
  }

  private void PositionGun() {
    
    Transform tr = activeGunModel.muzzle.transform;
    Transform mtr = muzzleFlash.transform;
    
    mtr.SetPositionAndRotation(tr.position, tr.rotation);
    mtr.Rotate(0, shootTimer * 1000, 0, Space.Self);
    mtr.localScale = tr.localScale;
    
    if (reloadTimer <= 0) {
      transform.localRotation = Quaternion.Lerp(shootRotation, originalRotation, 1 - shootTimer / (1 / guns[activeGun].fireRate));
      ammoText.text = guns[activeGun].ammoInMag + " / " + guns[activeGun].maxAmmoInMag;
      ammoLeftText.text = "" + guns[activeGun].ammoTotal;
    }
    else {
      ammoText.text =  "R / " + guns[activeGun].maxAmmoInMag;
      switch (true) {
        case true when reloadTimer > guns[activeGun].reloadTime * 0.75f: {
          gunTransform.localRotation = Quaternion.Lerp(gunTransform.localRotation, Quaternion.Euler(-90, 0, 0), 10 * Time.deltaTime / guns[activeGun].reloadTime * 2);
          gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].hipPos + Vector3.forward * .5f + occlusionOffset, 10 * guns[activeGun].reloadTime * Time.deltaTime / guns[activeGun].reloadTime * 2);      
        } break;
        case true when reloadTimer > guns[activeGun].reloadTime * 0.5f: {
          if (reloadTimer > guns[activeGun].reloadTime * 0.6f && !soundManager.SoundPlaying(reloadSounds[0])) soundManager.PlaySound(reloadSounds[0]);
          gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].hipPos + Vector3.down * .5f + Vector3.forward * .5f + occlusionOffset, 10 * Time.deltaTime / guns[activeGun].reloadTime * 2);
        } break;
        case true when reloadTimer > guns[activeGun].reloadTime * 0.25f: {
          if (reloadTimer > guns[activeGun].reloadTime * 0.4f && !soundManager.SoundPlaying(reloadSounds[1])) soundManager.PlaySound(reloadSounds[1]);
          gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].hipPos + Vector3.forward * .5f, 10 * Time.deltaTime / guns[activeGun].reloadTime * 2);
        } break;
        case true when reloadTimer > 0: {
          gunTransform.localRotation = Quaternion.Lerp(gunTransform.localRotation, Quaternion.Euler(0, 0, 0), 10 * Time.deltaTime / guns[activeGun].reloadTime * 2);
          gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].hipPos + occlusionOffset, 10 * Time.deltaTime / guns[activeGun].reloadTime * 2);
        } break;
      }
    }
  }

  private void Shoot() {
    if(reloadTimer >= 0 | shootTimer >= 0) return;
    if (guns[activeGun].ammoInMag <= 0) { OnReload(); return; }
    guns[activeGun].ammoInMag--;
      
    shootTimer = 1 / guns[activeGun].fireRate * (aimingDownSights ? 1.5f : 1f); // convert rps to time between rounds
    
    muzzleFlash.gameObject.SetActive(true);
    
    Transform tr = transform;
    
    soundManager.PlaySound(gunSounds.First(x => x.gun == guns[activeGun].ammoType).clip);
    
    for (int i = 0; i < guns[activeGun].bulletsPerShot; i++) {
      if (Physics.Raycast(camTransform.position, Quaternion.Euler(new Vector3(Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy))) * camTransform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("LevelGeom", "EnemyBody"))) {
        //camTransform.eulerAngles += new Vector3(-guns[activeGun].recoil * (aimingDownSights ? 0.5f : 1), 0, 0);
        tr.LookAt(hit.point);
        tr.eulerAngles += new Vector3(-guns[activeGun].recoil * guns[activeGun].bulletsPerShot, 0, 0);
        shootRotation = tr.localRotation;

        Transform ptr = particles.transform;

        if (hit.transform.CompareTag("Enemy")) { // hit enemy
          EnemyPartIdentifier enemy = hit.transform.GetComponent<EnemyPartIdentifier>();
          enemy.TakeDamage(guns[activeGun].damage);

          ptr.position = hit.point;
          ptr.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
          particles.Emit(3);
        }
        else if (hit.transform.CompareTag("Ground")) {
          Instantiate(bulletHole, hit.point + hit.normal * .001f, Quaternion.FromToRotation(Vector3.up, hit.normal));
          ptr.position = hit.point;
          ptr.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
          particles.Emit(3);
        }
        accuracy += guns[activeGun].inaccuracySpeed;
      }
      else {
        tr.eulerAngles += new Vector3(-guns[activeGun].recoil, 0, 0);
        shootRotation = tr.localRotation;
        accuracy += guns[activeGun].inaccuracySpeed;
      }
    }

    Vector3 camAngles = camTransform.eulerAngles;
    camAngles -= new Vector3(guns[activeGun].recoil * (aimingDownSights ? 0.5f : 1) * guns[activeGun].bulletsPerShot, 0, 0);
    camAngles = new Vector3(CameraLook.ClampAngle(camAngles.x, -90, 90), camAngles.y, camAngles.z);
    camTransform.eulerAngles = camAngles;
  }

  private void OnReload() {
    if (guns[activeGun].ammoTotal <= 0 || reloadTimer >= 0 || guns[activeGun].ammoInMag >= guns[activeGun].maxAmmoInMag) return;
    
    reloadTimer = guns[activeGun].reloadTime;
    
    guns[activeGun].ammoTotal -= guns[activeGun].maxAmmoInMag - guns[activeGun].ammoInMag;
    if (guns[activeGun].ammoTotal < 0) {
      guns[activeGun].ammoInMag = guns[activeGun].maxAmmoInMag + guns[activeGun].ammoTotal;
      guns[activeGun].ammoTotal = 0;
    }
    else guns[activeGun].ammoInMag = guns[activeGun].maxAmmoInMag;
  }

  private void AimDownSights() {
    if (!aimingDownSights || reloadTimer > 0) {
      portalCam.fieldOfView = Mathf.Lerp(portalCam.fieldOfView, 90, guns[activeGun].adsSpeed * Time.deltaTime);
      mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, 90, guns[activeGun].adsSpeed * Time.deltaTime);
      gunCam.fieldOfView = Mathf.Lerp(gunCam.fieldOfView, 90, guns[activeGun].adsSpeed * Time.deltaTime);
      gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].hipPos + occlusionOffset, guns[activeGun].adsSpeed * Time.deltaTime);
    }
    else {
      portalCam.fieldOfView = Mathf.Lerp(portalCam.fieldOfView, 60, guns[activeGun].adsSpeed * Time.deltaTime);
      mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, 60, guns[activeGun].adsSpeed * Time.deltaTime);
      gunCam.fieldOfView = Mathf.Lerp(gunCam.fieldOfView, 60, guns[activeGun].adsSpeed * Time.deltaTime);
      gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].adsPos + occlusionOffset, guns[activeGun].adsSpeed * Time.deltaTime);
    }
  }
}
