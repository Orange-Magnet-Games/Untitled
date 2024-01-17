using TMPro;
using UnityEngine;

public class GunController : MonoBehaviour {
  private PlayerController player;
  private Transform camTransform;
  private Camera mainCam, portalCam;
  private InputMaster input;
  
  public Gun gun;
  private GunModel activeGunModel;
  [SerializeField] private MuzzleFlash muzzleFlash;
  public BulletHole bulletHole;
  [SerializeField] private ParticleSystem particles;

  private float accuracy;
  private bool shooting, aimingDownSights;
  private float shootTimer, reloadTimer, fov;
  private Quaternion shootRotation = Quaternion.identity, originalRotation = Quaternion.identity;
  
  private TMP_Text ammoText, ammoLeftText;
  [SerializeField] private Transform gunTransform;
  private Crosshair crosshair;
  private void Start() {
    gun = Instantiate(gun);
    activeGunModel = Instantiate(gun.model, gunTransform).GetComponent<GunModel>();
    activeGunModel.transform.localPosition = gun.gunOffset;
    
    player = GameManager.instance.player;
    mainCam = GameManager.instance.mainCamera;
    portalCam = GameManager.instance.portalCamera;
    camTransform = mainCam.transform;
    
    input = player.input;

    input.Shooter.Shoot.performed += _ => shooting = true;
    input.Shooter.Shoot.canceled += _ => shooting = false;

    input.Shooter.Reload.performed += _ => OnReload();
    
    input.Shooter.ADS.performed += _ => aimingDownSights = true;
    input.Shooter.ADS.canceled += _ => aimingDownSights = false;

    originalRotation = transform.localRotation;

    ammoText = GameManager.instance.uiManager.ammoText;
    ammoLeftText = GameManager.instance.uiManager.ammoLeftText;
    crosshair = GameManager.instance.uiManager.crosshair;
  }

  private void Update() {
    shootTimer -= shootTimer >= 0 ? Time.deltaTime : 0;
    reloadTimer -= reloadTimer >= 0 ? Time.deltaTime : 0;
    accuracy = Mathf.Clamp(accuracy - (accuracy >= gun.bestAccuracy ? Time.deltaTime * gun.reAccurracySpeed : 0), gun.bestAccuracy, aimingDownSights ? gun.worstAccuracy / 2 : gun.worstAccuracy);
    crosshair.LineMovement = accuracy;
    
    // ReSharper disable twice Unity.PerformanceCriticalCodeInvocation
    if (shooting) Shoot();
    AimDownSights();

    Transform tr = activeGunModel.muzzle.transform;
    
    muzzleFlash.transform.SetPositionAndRotation(tr.position, tr.rotation);
    muzzleFlash.transform.Rotate(0, shootTimer / (1 / gun.fireRate) * 360, 0, Space.Self);

    

    if (reloadTimer <= 0) {
      transform.localRotation = Quaternion.Lerp(shootRotation, originalRotation, 1 - shootTimer / (1 / gun.fireRate));
      ammoText.text = gun.ammoInMag + " / " + gun.maxAmmoInMag;
      ammoLeftText.text = "" + gun.ammoTotal;
    }
    else {
      ammoText.text =  "R / " + gun.maxAmmoInMag;
      transform.Rotate(new Vector3(0, 0, 1), 1000 * Time.deltaTime, Space.Self);
    }
    
    
  }

  private void Shoot() {
    if(reloadTimer >= 0 | shootTimer >= 0) return;
    if (gun.ammoInMag <= 0) { OnReload(); return; }
    gun.ammoInMag--;
    
    shootTimer = 1 / gun.fireRate; // convert rps to time between rounds
    
    muzzleFlash.gameObject.SetActive(true);
    muzzleFlash.timeShown = 1 / gun.fireRate;
    
    if (!Physics.Raycast(camTransform.position, 
          Quaternion.Euler(Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy), Random.Range(-accuracy, accuracy)) * camTransform.forward, 
          out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("LevelGeom", "Enemy", "Portal"))) return;
    
    if (hit.transform.CompareTag("Portal")) return;
    
    Transform ptr = particles.transform;
    
    if (hit.transform.CompareTag("Enemy")) { // hit enemy
      EnemyController enemy = hit.transform.GetComponent<EnemyController>();
      enemy.TakeDamage(gun.damage);
      
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
    
    
    
    
    
    Transform tr = transform;
    tr.LookAt(hit.point);
    shootRotation = tr.localRotation;
    
    accuracy += gun.inaccuracySpeed;
  }

  private void OnReload() {
    if (gun.ammoTotal <= 0) return;
    
    reloadTimer = gun.reloadTime;
    
    gun.ammoTotal -= gun.maxAmmoInMag - gun.ammoInMag;
    if (gun.ammoTotal < 0) {
      gun.ammoInMag = gun.maxAmmoInMag + gun.ammoTotal;
      gun.ammoTotal = 0;
    }
    else gun.ammoInMag = gun.maxAmmoInMag;
  }

  private void AimDownSights() {
    if (!aimingDownSights || reloadTimer > 0) {
      portalCam.fieldOfView = Mathf.Lerp(portalCam.fieldOfView, 90, gun.adsSpeed * Time.deltaTime);
      mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, 90, gun.adsSpeed * Time.deltaTime);
      gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, gun.hipPos, gun.adsSpeed * Time.deltaTime);
    }
    else {
      portalCam.fieldOfView = Mathf.Lerp(portalCam.fieldOfView, 60, gun.adsSpeed * Time.deltaTime);
      mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, 60, gun.adsSpeed * Time.deltaTime);
      gunTransform.localPosition = Vector3.Lerp(gunTransform.localPosition, gun.adsPos, gun.adsSpeed * Time.deltaTime);
    }
  }
}
