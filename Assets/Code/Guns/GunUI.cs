using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunUI : MonoBehaviour { 
  [HideInInspector] public int gunIndex;
  [SerializeField] private Transform[] guns;
  private List<Image> gunImages;
  public Transform[] gunPivots;


  private void Start() {
    gunImages = new List<Image>();
    foreach (Transform gun in guns) gunImages.Add(gun.Find("Icon").GetComponent<Image>());
  }

  private void Update() { 
    guns[gunIndex].localScale = Vector3.Lerp(guns[gunIndex].localScale, Vector3.one * 1.5f, 10f * Time.deltaTime);
    gunImages[gunIndex].color = Color.Lerp(gunImages[gunIndex].color, Color.white * 0.25f, 10f * Time.deltaTime);
    gunImages[gunIndex].color = new Color(gunImages[gunIndex].color.r, gunImages[gunIndex].color.g, gunImages[gunIndex].color.b, 1);
    for (int i = 0; i < guns.Length; i++) {
      if (i == gunIndex) continue;
      guns[i].localScale = Vector3.Lerp(guns[i].localScale, Vector3.one, 10f * Time.deltaTime);
      gunImages[i].color = Color.Lerp(gunImages[i].color, Color.white, 10f * Time.deltaTime);


    }
  }
}
