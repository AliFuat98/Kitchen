using System;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour {
  public static KitchenGameManager Instance { get; private set; }

  public event EventHandler OnStateChanged;

  public event EventHandler<OnGamePausedToggledEventArgs> OnGamePausedToggled;

  public class OnGamePausedToggledEventArgs : EventArgs {
    public bool isGamePaused;
  }

  private enum State {
    WaitingToStart,
    CountdownToStart,
    GamePlaying,
    GameOver,
  }

  private State currentState;

  private State CurrentState {
    get { return currentState; }
    set {
      if (currentState != value) {
        currentState = value;
        OnStateChanged?.Invoke(this, new EventArgs());
        return;
      }
      currentState = value;
    }
  }

  private float waitingToStartTimer = 1f;
  private float countdownToStartTimer = 3f;
  private float gamePlayingTimer;
  [SerializeField] private float gamePlayingTimerMax = 20f;

  private bool isGamePaused = false;

  private void Awake() {
    CurrentState = State.WaitingToStart;
    Instance = this;
  }

  private void Start() {
    GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
  }

  private void GameInput_OnPauseAction(object sender, EventArgs e) {
    TogglePauseGame();
  }

  private void Update() {
    switch (CurrentState) {
      case State.WaitingToStart:
        waitingToStartTimer -= Time.deltaTime;
        if (waitingToStartTimer < 0) {
          CurrentState = State.CountdownToStart;
        }
        break;

      case State.CountdownToStart:
        countdownToStartTimer -= Time.deltaTime;
        if (countdownToStartTimer < 0) {
          CurrentState = State.GamePlaying;
          gamePlayingTimer = gamePlayingTimerMax;
        }
        break;

      case State.GamePlaying:
        gamePlayingTimer -= Time.deltaTime;
        if (gamePlayingTimer < 0) {
          CurrentState = State.GameOver;
        }
        break;

      case State.GameOver:
        break;
    }
  }

  public void TogglePauseGame() {
    if (isGamePaused) {
      // oyun durmuþ durumda => oynat
      Time.timeScale = 1f;
    } else {
      // oyun devam ediyor => durdur
      Time.timeScale = 0f;
    }

    isGamePaused = !isGamePaused;

    OnGamePausedToggled?.Invoke(this, new OnGamePausedToggledEventArgs {
      isGamePaused = isGamePaused
    });
  }

  public bool IsGamePlaying() {
    return CurrentState == State.GamePlaying;
  }

  public bool IsCountDownToStartActive() {
    return currentState == State.CountdownToStart;
  }

  public float GetCountDownToStartTimer() {
    return countdownToStartTimer;
  }

  public bool IsGameOver() {
    return currentState == State.GameOver;
  }

  public float GetGamePlayingTimerNormalized() {
    // max'dan çýkararak süreyi saydýðýndan dolayý 1 den çýkarýyoruz
    return 1 - (gamePlayingTimer / gamePlayingTimerMax);
  }
}