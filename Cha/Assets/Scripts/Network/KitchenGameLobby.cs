using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class KitchenGameLobby : MonoBehaviour {
  public static KitchenGameLobby Instance { get; private set; }

  public event EventHandler OnCreateLobbyStarted;

  public event EventHandler OnCreateLobbyFailedStarted;

  public event EventHandler OnJoinStarted;

  public event EventHandler OnJoinFailed;

  public event EventHandler OnQuickJoinFailedStarted;

  public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;

  public class OnLobbyListChangedEventArgs : EventArgs {
    public List<Lobby> lobbyList;
  }

  // girdiðimiz lobby
  private Lobby joinedLobby;

  private float heartBeatTimer;
  private float listLobbyTimer;

  private void Awake() {
    Instance = this;

    DontDestroyOnLoad(gameObject);
  }

  private void Start() {
    InitializeUnityAuthentication();
  }

  public async void InitializeUnityAuthentication() {
    if (UnityServices.State != ServicesInitializationState.Initialized) {
      // önceden baþlatýlmadýysa 2 kez baþlatmamak için

      // ayný pc de build alýnca id deðiþmesi için options geçiyoruz
      InitializationOptions options = new InitializationOptions();
      options.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

      // servisi baþlat
      await UnityServices.InitializeAsync(options);

      // giriþ yap
      await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
  }

  private void Update() {
    HandleHeartBeat();
    HandlePeriodicListLobbies();
  }

  private void HandleHeartBeat() {
    if (IsLobbyHost()) {
      heartBeatTimer -= Time.deltaTime;
      if (heartBeatTimer < 0) {
        float heartBeatTimerMax = 15f;
        heartBeatTimer = heartBeatTimerMax;

        LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
      }
    }
  }

  private void HandlePeriodicListLobbies() {
    if (joinedLobby != null || !AuthenticationService.Instance.IsSignedIn) {
      return;
    }
    listLobbyTimer -= Time.deltaTime;
    if (listLobbyTimer < 0) {
      float listLobbyTimerMax = 3f;
      listLobbyTimer = listLobbyTimerMax;
      ListLobbies();
    }
  }

  #region LOBBY

  private async void ListLobbies() {
    try {
      QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
        Filters = new List<QueryFilter> {
        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0",QueryFilter.OpOptions.GT)
      }
      };
      QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
      OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs {
        lobbyList = queryResponse.Results
      });
    } catch (LobbyServiceException e) {
      Debug.Log(e);
    }
  }

  private bool IsLobbyHost() {
    return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
  }

  public async void CreateLobby(string lobbyName, bool isPrivate) {
    OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
    try {
      joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYER_AMOUNT,
        new CreateLobbyOptions { IsPrivate = isPrivate }
      );

      KitchenGameMultiplayer.Instance.StartHost();
      Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
    } catch (LobbyServiceException e) {
      Debug.Log(e);
      OnCreateLobbyFailedStarted?.Invoke(this, EventArgs.Empty);
    }
  }

  public async void QuickJoin() {
    OnJoinStarted?.Invoke(this, EventArgs.Empty);
    try {
      joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

      KitchenGameMultiplayer.Instance.StartClient();
    } catch (LobbyServiceException e) {
      Debug.Log(e);
      OnQuickJoinFailedStarted?.Invoke(this, EventArgs.Empty);
    }
  }

  public async void JoinWithCode(string lobbyCode) {
    OnJoinStarted?.Invoke(this, EventArgs.Empty);
    try {
      joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

      KitchenGameMultiplayer.Instance.StartClient();
    } catch (LobbyServiceException e) {
      Debug.Log(e);
      OnJoinFailed?.Invoke(this, EventArgs.Empty);
    }
  }

  public async void JoinWithId(string lobbyId) {
    OnJoinStarted?.Invoke(this, EventArgs.Empty);
    try {
      joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

      KitchenGameMultiplayer.Instance.StartClient();
    } catch (LobbyServiceException e) {
      Debug.Log(e);
      OnJoinFailed?.Invoke(this, EventArgs.Empty);
    }
  }

  public async void DeleteLobby() {
    try {
      if (joinedLobby != null) {
        await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

        joinedLobby = null;
      }
    } catch (LobbyServiceException e) {
      Debug.Log(e);
    }
  }

  public async void LeaveLobby() {
    try {
      if (joinedLobby != null) {
        await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

        joinedLobby = null;
      }
    } catch (LobbyServiceException e) {
      Debug.Log(e);
    }
  }

  public async void KickPlayer(string playerID) {
    try {
      if (IsLobbyHost()) {
        await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerID);
      }
    } catch (LobbyServiceException e) {
      Debug.Log(e);
    }
  }

  public Lobby GetJoinedLobby() {
    return joinedLobby;
  }

  #endregion LOBBY
}