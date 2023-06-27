using System;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {

  /// progres de�i�ti�inde �al��acak event
  public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

  /// ocak yan�yorsa �al��acak event
  public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

  public class OnStateChangedEventArgs : EventArgs {
    public State state;
  }

  public enum State {
    Idle, // �zeri bo�
    Frying, // yan�yor
    Fried, // pi�mi� ama hala yan�yor
    Burned, // yanm��
  }

  [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
  [SerializeField] private BurningRecipeSO[] BurningRecipeSOArray;

  private NetworkVariable<State> xCurrentState = new(State.Idle);

  private State CurrentState {
    get { return xCurrentState.Value; }
    set {
      //if (xCurrentState.Value != value) {
      //  //eski veriden farkl� bir veri geliyorsa yani state de�i�imi var
      //  OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
      //    state = value
      //  });
      //}
      xCurrentState.Value = value;
    }
  }

  private NetworkVariable<float> xFryingTimer = new(0f);

  private float FryingTimer {
    get { return xFryingTimer.Value; }
    set {
      //if (xFryingTimer.Value != value) {
      //  OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
      //    progressNormalized = value / fryingRecipeSO.FryingTimerMax
      //  });
      //}
      xFryingTimer.Value = value;
    }
  }

  private NetworkVariable<float> xBurningTimer = new(0f);

  private float BurningTimer {
    get { return xBurningTimer.Value; }
    set {
      //if (xBurningTimer.Value != value) {
      //  OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
      //    progressNormalized = value / burningRecipeSO.BurningTimerMax
      //  });
      //}
      xBurningTimer.Value = value;
    }
  }

  private FryingRecipeSO fryingRecipeSO;
  private BurningRecipeSO burningRecipeSO;

  private void Start() {
    CurrentState = State.Idle;
  }

  public override void OnNetworkSpawn() {
    xFryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
    xBurningTimer.OnValueChanged += xBurningTimer_OnValueChanged;
    xCurrentState.OnValueChanged += xCurrentState_OnValueChanged;
  }

  private void FryingTimer_OnValueChanged(float previosValue, float newValue) {
    float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.FryingTimerMax : 1f;

    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
      progressNormalized = FryingTimer / fryingTimerMax
    });
  }

  private void xBurningTimer_OnValueChanged(float previosValue, float newValue) {
    float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.BurningTimerMax : 1f;

    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
      progressNormalized = BurningTimer / burningTimerMax
    });
  }

  private void xCurrentState_OnValueChanged(State previosValue, State newValue) {
    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
      state = CurrentState
    });
  }

  private void Update() {
    if (!IsServer) {
      return;
    }

    if (HasKitchenObject()) {
      switch (CurrentState) {
        case State.Idle:
          break;

        case State.Frying:

          FryingTimer += Time.deltaTime;
          if (FryingTimer > fryingRecipeSO.FryingTimerMax) {
            // pi�ti

            // �ncekini sil
            KitchenGameMultiplayer.Instance.DestroyKitchenObject(GetKitchenObject());

            // yenisini spawn et
            KitchenObject.SpwanKitchenObject(fryingRecipeSO.output, this);

            CurrentState = State.Fried;
            BurningTimer = 0f;

            // yanma tarfini al
            SetBuringRecipeSOClientRpc(
              KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO())
            );
          }
          break;

        case State.Fried:
          BurningTimer += Time.deltaTime;
          if (BurningTimer > burningRecipeSO.BurningTimerMax) {
            // yand�

            // �ncekini sil
            KitchenGameMultiplayer.Instance.DestroyKitchenObject(GetKitchenObject());

            // yenisini spawn et
            KitchenObject.SpwanKitchenObject(burningRecipeSO.output, this);

            CurrentState = State.Burned;
            BurningTimer = 0f;
          }
          break;

        case State.Burned:
          break;

        default:
          break;
      }
    }
  }

  public override void Interact(Player player) {
    if (!HasKitchenObject()) {
      // kutunun �zeri bo�

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
          // oyuncunun elinde pi�irelebilir bir malzeme var

          // malzemeyi kutunun �zerine b�rak
          KitchenObject kitchenObject = player.GetKitchenObject();
          kitchenObject.SetKitchenObjectParent(this);

          InteractLogicPlaceObjectOnCounterServerRpc(
            KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO())
          );
        } else {
          // oyuncunun elinde kesilebilir bir malzeme yok

          // ---
        }
      } else {
        // oyuncunun eli bo�

        // ---
      }
    } else {
      // kutunun �zeri dolu

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
          // oyuncunun elindeki malzeme bir tabak => out parametresiyle ula��labilir

          if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
            // kutunun �zerindeki malzeme taba�a eklenebilir. => eklendi

            // kutunun �zerindekini yok et
            KitchenObject.DestroyKitchenObject(GetKitchenObject());

            SetStateIdleServerRpc();
          }
        } else {
          // oyuncu tabak hari� ba�ka bir �ey ta��yor

          // ---
        }
      } else {
        // oyuncunun eli bo�

        // kutunun �zerindeki malzemeyi oyuncuya ver
        GetKitchenObject().SetKitchenObjectParent(player);

        SetStateIdleServerRpc();
      }
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void SetStateIdleServerRpc() {
    CurrentState = State.Idle;
    FryingTimer = 0;
    BurningTimer = 0;
  }

  [ServerRpc(RequireOwnership = false)]
  private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex) {
    // tarifeyi update'te kullanmak i�in �ek
    SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);

    // state'i ba�lat
    CurrentState = State.Frying;

    // s�reyi s�f�rla
    FryingTimer = 0f;
    BurningTimer = 0f;
  }

  [ClientRpc]
  private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex) {
    KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
    fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
  }

  [ClientRpc]
  private void SetBuringRecipeSOClientRpc(int kitchenObjectSOIndex) {
    KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
    burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
  }

  /// ocaktaki et pi�mi� yanmaya do�ru mu gidiyor
  public bool IsFried() {
    return CurrentState == State.Fried;
  }

  /// gelen malzemenin input oldu�u tarifi ver
  private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (var fryingRecipeSO in fryingRecipeSOArray) {
      if (fryingRecipeSO.input == inputKitchenObjectSO) {
        return fryingRecipeSO;
      }
    }
    return null;
  }

  /// gelen malzemenin input oldu�u tarifi ver
  private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (var burningRecipeSO in BurningRecipeSOArray) {
      if (burningRecipeSO.input == inputKitchenObjectSO) {
        return burningRecipeSO;
      }
    }
    return null;
  }

  /// gelen malzemenin bir sonraki halini d�n
  private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
    var fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
    if (fryingRecipeSO == null) {
      // gelen malzeme tarifte yok
      return null;
    }

    return fryingRecipeSO.output;
  }

  /// gelen malzemenin tarifi var m�?
  private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
    var fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
    return fryingRecipeSO != null;
  }
}