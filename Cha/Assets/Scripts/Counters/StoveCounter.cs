using System;
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

  private State currentState;

  private State CurrentState {
    get { return currentState; }
    set {
      if (currentState != value) {
        //eski veriden farklý bir veri geliyorsa yani state deðiþimi var
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
          state = value
        });
      }
      currentState = value;
    }
  }

  private float fryingTimer;

  private float FryingTimer {
    get { return fryingTimer; }
    set {
      if (fryingTimer != value) {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
          progressNormalized = value / fryingRecipeSO.FryingTimerMax
        });
      }
      fryingTimer = value;
    }
  }

  private float burningTimer;

  private float BurningTimer {
    get { return burningTimer; }
    set {
      if (burningTimer != value) {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
          progressNormalized = value / burningRecipeSO.BurningTimerMax
        });
      }
      burningTimer = value;
    }
  }

  private FryingRecipeSO fryingRecipeSO;
  private BurningRecipeSO burningRecipeSO;

  private void Start() {
    CurrentState = State.Idle;
  }

  private void Update() {
    if (HasKitchenObject()) {
      switch (CurrentState) {
        case State.Idle:
          break;

        case State.Frying:

          FryingTimer += Time.deltaTime;
          if (FryingTimer > fryingRecipeSO.FryingTimerMax) {
            // piþti

            // öncekini sil
            GetKitchenObject().DestroyItelf();

            // yenisini spawn et
            KitchenObject.SpwanKitchenObject(fryingRecipeSO.output, this);

            CurrentState = State.Fried;
            BurningTimer = 0f;

            // yanma tarfini al
            burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
          }
          break;

        case State.Fried:
          BurningTimer += Time.deltaTime;
          if (BurningTimer > burningRecipeSO.BurningTimerMax) {
            // yandý

            // öncekini sil
            GetKitchenObject().DestroyItelf();

            // yenisini spawn et
            KitchenObject.SpwanKitchenObject(burningRecipeSO.output, this);

            CurrentState = State.Burned;

            burningTimer = 0f;
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
          player.GetKitchenObject().SetKitchenObjectParent(this);

          // tarifeyi update'te kullanmak için çek
          fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

          // state'i baþlat
          CurrentState = State.Frying;
          // süreyi sýfýrla
          FryingTimer = 0f;
          burningTimer = 0f;
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

        // ---
      } else {
        // oyuncunun eli boþ

        // kutunun üzerindeki malzemeyi oyuncuya ver
        GetKitchenObject().SetKitchenObjectParent(player);

        CurrentState = State.Idle;
        FryingTimer = 0f;
        burningTimer = 0f;
      }
    }
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