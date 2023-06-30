using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameLobby : MonoBehaviour {
  private const string KEY_RELAY_JOIN_CODE = "relayJoinCode";
  public static KitchenGameLobby Instance { get; private set; }

  public event EventHandler OnCreateLobbyStarted;

  public event EventHandler OnCreateLobbyFailedStarted;

  public event EventHandler OnJoinStarted;

  public event EventHandler OnQuickJoinFailedStarted;

  public event EventHandler OnJoinFailed;

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

  private void HandlePeriodicListLobbies() {
    if (joinedLobby != null ||
      UnityServices.State != ServicesInitializationState.Initialized ||
      !AuthenticationService.Instance.IsSignedIn ||
      SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString()) {
      return;
    }
    listLobbyTimer -= Time.deltaTime;
    if (listLobbyTimer < 0f) {
      float listLobbyTimerMax = 3f;
      listLobbyTimer = listLobbyTimerMax;
      ListLobbies();
    }
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

  #region LOBBY

  private async void ListLobbies() {
    Debug.Log("ListLobbies");
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

  private async Task<Allocation> AllocateRelay() {
    try {
      Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYER_AMOUNT - 1);

      return allocation;
    } catch (RelayServiceException e) {
      Debug.Log(e);
      return default;
    }
  }

  private async Task<string> GetRelayJoinCode(Allocation allocation) {
    try {
      string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

      return relayJoinCode;
    } catch (RelayServiceException e) {
      Debug.Log(e);
      return default;
    }
  }

  public async Task<JoinAllocation> JoinRelay(string joinCode) {
    try {
      JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

      return joinAllocation;
    } catch (RelayServiceException e) {
      Debug.Log(e);
      return default;
    }
  }

  public async void CreateLobby(string lobbyName, bool isPrivate) {
    OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
    try {
      joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYER_AMOUNT,
        new CreateLobbyOptions { IsPrivate = isPrivate }
      );

      Allocation allocation = await AllocateRelay();

      string relayJoincode = await GetRelayJoinCode(allocation);

      await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
        Data = new Dictionary<string, DataObject> {
          {KEY_RELAY_JOIN_CODE ,new DataObject(DataObject.VisibilityOptions.Member, relayJoincode) }
        }
      });

      NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

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

      string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
      JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

      NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

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

      string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
      JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

      NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

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

      string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
      JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

      NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

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