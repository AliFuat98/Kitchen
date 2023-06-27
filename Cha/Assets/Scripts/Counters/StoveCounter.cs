using System;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {

  /// progres deðiþtiðinde çalýþacak event
  public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

  /// ocak yanýyorsa çalýþacak event
  public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

  public class OnStateChangedEventArgs : EventArgs {
    public State state;
  }

  public enum State {
    Idle, // üzeri boþ
    Frying, // yanýyor
    Fried, // piþmiþ ama hala yanýyor
    Burned, // yanmýþ
  }

  [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
  [SerializeField] private BurningRecipeSO[] BurningRecipeSOArray;

  private NetworkVariable<State> xCurrentState = new(State.Idle);

  private State CurrentState {
    get { return xCurrentState.Value; }
    set {
      //if (xCurrentState.Value != value) {
      //  //eski veriden farklý bir veri geliyorsa yani state deðiþimi var
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
            // piþti

            // öncekini sil
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
            // yandý

            // öncekini sil
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
      // kutunun üzeri boþ

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
          // oyuncunun elinde piþirelebilir bir malzeme var

          // malzemeyi kutunun üzerine býrak
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
        // oyuncunun eli boþ

        // ---
      }
    } else {
      // kutunun üzeri dolu

      if (player.HasKitchenObject()) {
        // oyuncunun elinde malzeme var

        if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
          // oyuncunun elindeki malzeme bir tabak => out parametresiyle ulaþýlabilir

          if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
            // kutunun üzerindeki malzeme tabaða eklenebilir. => eklendi

            // kutunun üzerindekini yok et
            KitchenObject.DestroyKitchenObject(GetKitchenObject());

            SetStateIdleServerRpc();
          }
        } else {
          // oyuncu tabak hariç baþka bir þey taþýyor

          // ---
        }
      } else {
        // oyuncunun eli boþ

        // kutunun üzerindeki malzemeyi oyuncuya ver
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
    // tarifeyi update'te kullanmak için çek
    SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);

    // state'i baþlat
    CurrentState = State.Frying;

    // süreyi sýfýrla
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

  /// ocaktaki et piþmiþ yanmaya doðru mu gidiyor
  public bool IsFried() {
    return CurrentState == State.Fried;
  }

  /// gelen malzemenin input olduðu tarifi ver
  private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (var fryingRecipeSO in fryingRecipeSOArray) {
      if (fryingRecipeSO.input == inputKitchenObjectSO) {
        return fryingRecipeSO;
      }
    }
    return null;
  }

  /// gelen malzemenin input olduðu tarifi ver
  private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (var burningRecipeSO in BurningRecipeSOArray) {
      if (burningRecipeSO.input == inputKitchenObjectSO) {
        return burningRecipeSO;
      }
    }
    return null;
  }

  /// gelen malzemenin bir sonraki halini dön
  private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
    var fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
    if (fryingRecipeSO == null) {
      // gelen malzeme tarifte yok
      return null;
    }

    return fryingRecipeSO.output;
  }

  /// gelen malzemenin tarifi var mý?
  private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
    var fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
    return fryingRecipeSO != null;
  }
}