using System;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour {
  public static KitchenGameManager Instance { get; private set; }

  public event EventHandler OnStateChanged;

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

  private void Awake() {
    CurrentState = State.WaitingToStart;
    Instance = this;
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