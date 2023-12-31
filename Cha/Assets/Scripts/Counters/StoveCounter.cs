using System;
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

  private State xState;

  private State CurrentState {
    get { return xState; }
    set {
      if (xState != value) {
        //eski veriden farkl� bir veri geliyorsa yani state de�i�imi var
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
          state = value
        });
      }
      xState = value;
    }
  }

  private float xFTimer;

  private float FryingTimer {
    get { return xFTimer; }
    set {
      if (xFTimer != value) {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
          progressNormalized = value / fryingRecipeSO.FryingTimerMax
        });
      }
      xFTimer = value;
    }
  }

  private float xBTimer;

  private float BurningTimer {
    get { return xBTimer; }
    set {
      if (xBTimer != value) {
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
          progressNormalized = value / burningRecipeSO.BurningTimerMax
        });
      }
      xBTimer = value;
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
            // pi�ti

            // �ncekini sil
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
            // yand�

            // �ncekini sil
            GetKitchenObject().DestroyItelf();

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
          player.GetKitchenObject().SetKitchenObjectParent(this);

          // tarifeyi update'te kullanmak i�in �ek
          fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

          // state'i ba�lat
          CurrentState = State.Frying;
          // s�reyi s�f�rla
          FryingTimer = 0f;
          BurningTimer = 0f;
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
            GetKitchenObject().DestroyItelf();

            CurrentState = State.Idle;
            FryingTimer = 0f;
            BurningTimer = 0f;
          }
        } else {
          // oyuncu tabak hari� ba�ka bir �ey ta��yor

          // ---
        }
      } else {
        // oyuncunun eli bo�

        // kutunun �zerindeki malzemeyi oyuncuya ver
        GetKitchenObject().SetKitchenObjectParent(player);

        CurrentState = State.Idle;
        FryingTimer = 0f;
        BurningTimer = 0f;
      }
    }
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