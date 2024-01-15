using System;
using TMPro;
using UnityEngine;

public class GunController : MonoBehaviour {
  private PlayerController player;
  private Transform cam;
  private InputMaster input;
  
  public Gun gun;
  [SerializeField] private GunModel activeGunModel;
  [SerializeField] private MuzzleFlash muzzleFlash;
  public BulletHole bulletHole;
  [SerializeField] private ParticleSystem particles;
  
  private bool shooting, aimingDownSights;
  private float shootTimer, reloadTimer;
  private Quaternion shootRotation = Quaternion.identity, originalRotaion = Quaternion.identity;

  private TMP_Text ammoText;
  private void Start() {
    gun = Instantiate(gun);
    
    player = GameManager.instance.player;
    cam = GameManager.instance.mainCamera.transform;
    input = player.input;

    input.Shooter.Shoot.performed += _ => shooting = true;
    input.Shooter.Shoot.canceled += _ => shooting = false;

    input.Shooter.Reload.performed += _ => OnReload();
    
    input.Shooter.ADS.performed += _ => aimingDownSights = true;
    input.Shooter.ADS.canceled += _ => aimingDownSights = false;

    originalRotaion = transform.localRotation;

    ammoText = GameManager.instance.uiManager.ammoText;
  }

  private void Update() {
    shootTimer -= shootTimer >= 0 ? Time.deltaTime : 0;
    reloadTimer -= reloadTimer >= 0 ? Time.deltaTime : 0;
    
    // ReSharper disable twice Unity.PerformanceCriticalCodeInvocation
    if (shooting) Shoot();
    //if (aimingDownSights) AimDownSights();

    Transform tr = activeGunModel.muzzle.transform;
    
    muzzleFlash.transform.SetPositionAndRotation(tr.position, tr.rotation);
    muzzleFlash.transform.Rotate(0, shootTimer / (1 / gun.fireRate) * 360, 0, Space.Self);

    

    if (reloadTimer <= 0) {
      transform.localRotation = Quaternion.Lerp(shootRotation, originalRotaion, 1 - shootTimer / (1 / gun.fireRate));
      ammoText.text = gun.ammoInMag + " / " + gun.maxAmmoInMag;
    }
    else {
      ammoText.text =  "R / " + gun.maxAmmoInMag;
      transform.Rotate(new Vector3(1, 0, 0), 1000 * Time.deltaTime, Space.Self);
    }
    
    
  }

  private void Shoot() {
    if(reloadTimer >= 0 | shootTimer >= 0) return;
    if (gun.ammoInMag <= 0) { OnReload(); return; }
    gun.ammoInMag--;
    
    shootTimer = 1 / gun.fireRate; // convert rps to time between rounds
    
    muzzleFlash.gameObject.SetActive(true);
    muzzleFlash.timeShown = 1 / gun.fireRate;
    
    if (!Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("LevelGeom", "Enemy", "Portal"))) return;

    if (hit.transform.CompareTag("Portal")) return;
    
    Transform ptr = particles.transform;
    
    if (hit.transform.CompareTag("Enemy")) { // hit enemy
      EnemyController enemy = hit.transform.GetComponent<EnemyController>();
      enemy.TakeDamage(gun.damage);
      
      ptr.position = hit.point;
      ptr.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
      particles.Emit(3);
      // some more shit later
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
  }

  private void OnReload() {
    reloadTimer = gun.reloadTime;
    
    gun.ammoTotal -= gun.maxAmmoInMag - gun.ammoInMag;
    gun.ammoInMag = gun.maxAmmoInMag;
  }

  /*private void AimDownSights() {
    throw new NotImplementedException();
  }*/
}
