using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameManager : NetworkBehaviour {
  public static KitchenGameManager Instance { get; private set; }

  public event EventHandler OnStateChanged;

  public event EventHandler<OnGamePausedToggledEventArgs> OnGamePausedToggled;

  public class OnGamePausedToggledEventArgs : EventArgs {
    public bool isGamePaused;
  }

  public event EventHandler OnLocalPlayerReadyChanged;

  private enum State {
    WaitingToStart,
    CountdownToStart,
    GamePlaying,
    GameOver,
  }

  private Dictionary<ulong, bool> playerReadyDictionary;

  private NetworkVariable<State> xCurrentState = new(State.WaitingToStart);
  private bool isLocalPlayerReady;

  private State CurrentState {
    get { return xCurrentState.Value; }
    set {
      //if (xCurrentState.Value != value) {
      //  xCurrentState.Value = value;
      //  OnStateChanged?.Invoke(this, new EventArgs());
      //  return;
      //}
      xCurrentState.Value = value;
    }
  }

  private NetworkVariable<float> countdownToStartTimer = new(3f);
  private NetworkVariable<float> gamePlayingTimer = new(0f);

  [SerializeField] private float gamePlayingTimerMax = 10f;

  private bool isGamePaused = false;

  public override void OnNetworkSpawn() {
    xCurrentState.OnValueChanged += CurrentState_OnValueChanged;
  }

  private void CurrentState_OnValueChanged(State previousState, State nextState) {
    OnStateChanged?.Invoke(this, new EventArgs());
  }

  private void Awake() {
    CurrentState = State.WaitingToStart;
    Instance = this;
    playerReadyDictionary = new();
  }

  private void Start() {
    GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
    GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
  }

  private void GameInput_OnInteractAction(object sender, EventArgs e) {
    if (CurrentState == State.WaitingToStart) {
      isLocalPlayerReady = true;
      OnLocalPlayerReadyChanged?.Invoke(this, new EventArgs());

      SetPlayerReadyServerRpc();
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
    playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

    bool allClientsAreReady = true;
    foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
      if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]) {
        // this player is not ready
        allClientsAreReady = false;
      }
    }

    if (allClientsAreReady) {
      CurrentState = State.CountdownToStart;
    }
  }

  private void GameInput_OnPauseAction(object sender, EventArgs e) {
    TogglePauseGame();
  }

  private void Update() {
    if (!IsServer) {
      return;
    }

    switch (CurrentState) {
      case State.WaitingToStart:
        break;

      case State.CountdownToStart:
        countdownToStartTimer.Value -= Time.deltaTime;
        if (countdownToStartTimer.Value < 0) {
          CurrentState = State.GamePlaying;
          gamePlayingTimer.Value = gamePlayingTimerMax;
        }
        break;

      case State.GamePlaying:
        gamePlayingTimer.Value -= Time.deltaTime;
        if (gamePlayingTimer.Value < 0) {
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

  public bool IsLocalPlayerReady() {
    return isLocalPlayerReady;
  }

  public bool IsGamePlaying() {
    return CurrentState == State.GamePlaying;
  }

  public bool IsCountDownToStartActive() {
    return CurrentState == State.CountdownToStart;
  }

  public float GetCountDownToStartTimer() {
    return countdownToStartTimer.Value;
  }

  public bool IsGameOver() {
    return CurrentState == State.GameOver;
  }

  public float GetGamePlayingTimerNormalized() {
    // max'dan çýkararak süreyi saydýðýndan dolayý 1 den çýkarýyoruz
    return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
  }
}