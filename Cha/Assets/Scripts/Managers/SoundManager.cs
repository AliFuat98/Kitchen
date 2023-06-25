using UnityEngine;

public class SoundManager : MonoBehaviour {
  private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "soundEffectVolume";

  public static SoundManager Instance { get; private set; }

  private void Awake() {
    Instance = this;

    float defaultVolume = 1f;
    volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, defaultVolume);
  }

  [SerializeField] private AudioClipRefsSO audioClipRefsSO;

  private float volume;

  private void Start() {
    DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
    DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
    CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
    Player.Instance.OnPickedSomething += Player_OnPickedSomething;
    BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
    TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
  }

  public void ChangeVolume() {
    volume += .1f;
    if (volume > 1f) {
      volume = 0f;
    }
    PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
    PlayerPrefs.Save();
  }

  public float getVolume() {
    return volume;
  }

  public void PlayFootstedSound(Vector3 posiiton, float volume) {
    PlaySound(audioClipRefsSO.footStep, posiiton, volume);
  }

  private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e) {
    PlaySound(audioClipRefsSO.trash, ((TrashCounter)sender).transform.position);
  }

  private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e) {
    PlaySound(audioClipRefsSO.objectDrop, ((BaseCounter)sender).transform.position);
  }

  private void Player_OnPickedSomething(object sender, System.EventArgs e) {
    PlaySound(audioClipRefsSO.objectPickUp, Player.Instance.transform.position);
  }

  private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e) {
    CuttingCounter cuttingCounter = (CuttingCounter)sender;
    PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
  }

  private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e) {
    PlaySound(audioClipRefsSO.deliverySuccess, DeliveryManager.Instance.transform.position);
  }

  private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e) {
    PlaySound(audioClipRefsSO.deliveryFail, DeliveryManager.Instance.transform.position);
  }

  private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
    PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
  }

  private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f) {
    AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);
  }
}