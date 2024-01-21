using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour {
  private PlayerController player;
  private Transform camTransform;
  private Camera mainCam, portalCam;
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
  
  private float accuracy;
  private bool shooting, aimingDownSights;
  private float shootTimer, reloadTimer, fov;
  private Quaternion shootRotation = Quaternion.identity, originalRotation = Quaternion.identity;
  [HideInInspector] public Vector3 occlusionOffset = Vector3.zero;
  
  private TMP_Text ammoText, ammoLeftText;
  [SerializeField] private Transform gunTransform;
  private Crosshair crosshair;

  [SerializeField] private int[] startingGuns = new int[2];
  private void Start() {
    guns[0] = Instantiate(allPossibleGuns[startingGuns[0]]);
    guns[1] = Instantiate(allPossibleGuns[startingGuns[1]]);
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

    input.Shooter.WeaponScroll.performed += SwitchWeapon;
    
    originalRotation = transform.localRotation;

    ammoText = GameManager.instance.uiManager.ammoText;
    ammoLeftText = GameManager.instance.uiManager.ammoLeftText;
    crosshair = GameManager.instance.uiManager.crosshair;
    uiGuns = GameManager.instance.uiManager.guns;
    
    UpdateUIWeapons();
    
    uiGuns.gunIndex = activeGun;

  }

  private void SwitchWeapon(InputAction.CallbackContext ctx) {
    
    
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
      if (uiGuns.gunPivots[i].childCount > 0) Destroy(uiGuns.gunPivots[i].GetChild(i).gameObject);

      GameObject gj = Instantiate(guns[i].model, uiGuns.gunPivots[i]);
      gj.transform.localScale = guns[i].gunUIScale;
    }
  }
  
  private void Update() {
    shootTimer -= shootTimer >= 0 ? Time.deltaTime : 0;
    reloadTimer -= reloadTimer >= 0 ? Time.deltaTime : 0;
    accuracy = Mathf.Clamp(accuracy - (accuracy >= guns[activeGun].bestAccuracy ? Time.deltaTime * guns[activeGun].reAccurracySpeed : 0), guns[activeGun].bestAccuracy, aimingDownSights ? guns[activeGun].worstAccuracy / 2 : guns[activeGun].worstAccuracy);
    crosshair.LineMovement = accuracy;

    
    // ReSharper disable twice Unity.PerformanceCriticalCodeInvocation
    if (shooting) Shoot();
    AimDownSights();

    Transform tr = activeGunModel.muzzle.transform;
    Transform mtr = muzzleFlash.transform;
    
    mtr.SetPositionAndRotation(tr.position, tr.rotation);
    mtr.Rotate(0, shootTimer / (1 / guns[activeGun].fireRate) * 360, 0, Space.Self);
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
          gunTransform.localRotation = Quaternion.Lerp(gunTransform.localRotation, Quaternion.Euler(-90, 0, 0), 10 * Time.deltaTime / guns[activeGun].reloadTime);
          gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].hipPos + Vector3.forward * .5f + occlusionOffset, 10 * guns[activeGun].reloadTime * Time.deltaTime / guns[activeGun].reloadTime);
          break;
        }
        case true when reloadTimer > guns[activeGun].reloadTime * 0.5f: {
          gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].hipPos + Vector3.down * .5f + Vector3.forward * .5f + occlusionOffset, 10 * Time.deltaTime / guns[activeGun].reloadTime);
          break;
        }
        case true when reloadTimer > guns[activeGun].reloadTime * 0.25f: {
          gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].hipPos + Vector3.forward * .5f, 10 * Time.deltaTime / guns[activeGun].reloadTime);
          break;
        }
        case true when reloadTimer > 0: {
          gunTransform.localRotation = Quaternion.Lerp(gunTransform.localRotation, Quaternion.Euler(0, 0, 0), 10 * Time.deltaTime / guns[activeGun].reloadTime);
          gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].hipPos + occlusionOffset, 10 * Time.deltaTime / guns[activeGun].reloadTime);
          break;
        }
      }
    }
  }

  private void Shoot() {
    if(reloadTimer >= 0 | shootTimer >= 0) return;
    if (guns[activeGun].ammoInMag <= 0) { OnReload(); return; }
    guns[activeGun].ammoInMag--;

    shootTimer = 1 / guns[activeGun].fireRate * (aimingDownSights ? 1.5f : 1f); // convert rps to time between rounds
    
    muzzleFlash.gameObject.SetActive(true);
    for (int i = 0; i < guns[activeGun].bulletsPerShot; i++) {
      if (!Physics.Raycast(camTransform.position,
            Quaternion.Euler(Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy)) * camTransform.forward,
            out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("LevelGeom", "Enemy", "Portal"))) continue;
      
      Transform tr = transform;
      tr.LookAt(hit.point);
      shootRotation = tr.localRotation;
      
      accuracy += guns[activeGun].inaccuracySpeed;
      camTransform.eulerAngles += new Vector3(-guns[activeGun].recoil * (aimingDownSights ? 0.5f : 1), 0, 0);
      
      if (hit.transform.CompareTag("Portal")) continue;

      Transform ptr = particles.transform;

      if (hit.transform.CompareTag("Enemy")) { // hit enemy
        EnemyController enemy = hit.transform.GetComponent<EnemyController>();
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
    }
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
      gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].hipPos + occlusionOffset, guns[activeGun].adsSpeed * Time.deltaTime);
    }
    else {
      portalCam.fieldOfView = Mathf.Lerp(portalCam.fieldOfView, 60, guns[activeGun].adsSpeed * Time.deltaTime);
      mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, 60, guns[activeGun].adsSpeed * Time.deltaTime);
      gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, guns[activeGun].adsPos + occlusionOffset, guns[activeGun].adsSpeed * Time.deltaTime);
    }
  }
}
