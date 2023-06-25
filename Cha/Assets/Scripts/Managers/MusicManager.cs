using UnityEngine;

public class MusicManager : MonoBehaviour {
  private const string PLAYER_PREFS_MUSIC_VOLUME = "musicVolume";

  public static MusicManager Instance { get; private set; }

  private AudioSource audioSource;

  private float volume;

  private void Awake() {
    Instance = this;
    audioSource = GetComponent<AudioSource>();

    float defaultVolume = 0.5f;
    volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, defaultVolume);
    audioSource.volume = volume;
  }

  public void ChangeVolume() {
    volume += .1f;
    if (volume > 1f) {
      volume = 0f;
    }
    audioSource.volume = volume;

    PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
    PlayerPrefs.Save();
  }

  public float getVolume() {
    return volume;
  }
}