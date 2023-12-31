using UnityEngine;

public class PlayerSounds : MonoBehaviour {
  private Player player;

  private float footstepTimer;
  private float footstepTimerMax = 0.1f;

  private void Awake() {
    player = gameObject.GetComponent<Player>();
  }

  private void Start() {
  }

  private void Update() {
    footstepTimer -= Time.deltaTime;
    if (footstepTimer < 0f) {
      footstepTimer = footstepTimerMax;

      if (player.IsWalking()) {
        // oyuncu yürüyor

        // ses oynatılabilir
        float volume = 1f;
        SoundManager.Instance.PlayFootstedSound(transform.position, volume);
      }
    }
  }
}