using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  [SerializeField] private AudioSource sourcePrefab;
  [SerializeField] private AudioSource musicPlayer;
  private readonly List<AudioSource> sources = new List<AudioSource>();
  [SerializeField] private AudioClip[] songs;
  public bool musicPlaying;
  private float volume;
  private Transform player;
  private void Start() {
    player = GameManager.instance.player.transform;
    SetVolume(0.1f);
    musicPlayer.clip = songs[Random.Range(0, songs.Length)];
    musicPlayer.Play();
  }
  public void SetVolume(float vol) {
    volume = vol;
    if (!musicPlaying) return;
    musicPlayer.volume = volume;
    
  }
  private void Update() {
    if (sources.Count > 0) {
      foreach (AudioSource src in sources.Where(src => !src.isPlaying)) {
        sources.Remove(src);
        Destroy(src.gameObject);
      }
    }
    MusicManagement();
  }

  public bool SoundPlaying(AudioClip clip) => sources.Any(x => x.clip = clip);
  
  private void MusicManagement() {
    if(!musicPlaying) { musicPlayer.Stop(); return; }

    if (musicPlayer.isPlaying) return;
    
    musicPlayer.clip = songs[Random.Range(0, songs.Length)];
    musicPlayer.Play();
    musicPlayer.volume = volume;

  }

  public void PlaySound(AudioClip sound) {
    AudioSource src = Instantiate(sourcePrefab);
    src.clip = sound;
    src.Play();
    src.volume = volume;
    src.pitch = Random.Range(0.75f, 1.25f);
    sources.Add(src);
  }

}
