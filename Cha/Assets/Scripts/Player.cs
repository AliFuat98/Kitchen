using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent {

  /// Singleton pattern
  public static Player LocalInstance { get; private set; }

  /// <summary>
  /// bir oyuncu sahnede spawn olduðunda çalýþacak event
  /// static yapmamýzýn sebebide eventin class'ýn kendine ait olmasý
  /// spesifik bir player'a deðil
  /// </summary>
  public static event EventHandler OnAnyPlayerSpawned;

  public static event EventHandler OnAnyPickSomething;

  public static void ResetStaticData() {
    OnAnyPlayerSpawned = null;
    OnAnyPickSomething = null;
  }

  /// <summary>
  /// oyuncu seçili kutuyu deðiþtirdiðinde çalýþacak
  /// burada Publisher'ýmýz Player
  /// subs'lar ise kutular olucak. Kutular bu eventi dinlemede bekliyor.
  /// Deðiþim olduðunda tüm sub olan kutular atadýklarý fonksiyonu execute edicek (hide or show)
  /// hangi kutuyu seçtiðimizi sub olan kutularýn anlamasý içinde argüman olarak yolluyoruz
  /// </summary>
  public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

  public class OnSelectedCounterChangedEventArgs : EventArgs {
    public BaseCounter selectedCounter;
  }

  /// ses için event
  public event EventHandler OnPickedSomething;

  /// HIZ
  [SerializeField] private float moveSpeed = 7f;

  /// kutularý raycast ile hit yaparken maskeleme yapmak için
  [SerializeField] private LayerMask countersLayerMask;

  /// çarpýþmalar için maske
  [SerializeField] private LayerMask collisionsLayerMask;

  /// malzemeyi nerde spawn edicez = oyuncunun malzemeyi tutma noktasý
  [SerializeField] private Transform kitchenObjectHoldPoint;

  /// oyuncularýn spwan olucaklarý noktalar
  [SerializeField] private List<Vector3> spawnPositionList;

  /// oyuncunun üzerindeki malzeme
  private KitchenObject kitchenObject;

  /// Animation
  private bool isWalking;

  /// <summary>
  /// hareket etmeyi býraktýðýmýzda en son interact ettiðimiz yönü kaybetmemek için
  /// mesela kutunun önüne kadar geldin hareketi kestin son yön tekrar sýfýrlanýyor sýfýrlanmadan önceki deðeri tutar
  /// </summary>
  private Vector3 lastInteractDir;

  /// <summary>
  /// En son seçili olan kutuyu tutar.
  /// boþ arazide geziyorsak yakýnýmýzda kutu yoksa bu deðer null oluyor
  /// </summary>
  private BaseCounter selectedCounter;

  GameInput gameInputInstance;

  public override void OnNetworkSpawn() {
    if (IsOwner) {
      // þuan local player'dayýz

      LocalInstance = this;
    }
    transform.position = spawnPositionList[(int)OwnerClientId];
    OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
  }

  private void Start() {
    gameInputInstance = GameInput.Instance;

    // interaction tuþuna (E and F) tuþuna basýnca çalýþan event'e sub oluyoruz

    // E basýnca GameInput_OnInteratAction çalýþacak
    gameInputInstance.OnInteractAction += GameInput_OnInteratAction;

    // F basýnca GameInput_OnInteractAlternateAction çalýþacak
    gameInputInstance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
  }

  private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
    if (!KitchenGameManager.Instance.IsGamePlaying()) {
      return;
    }

    if (selectedCounter != null) {
      selectedCounter.InteractAlternate(this);
    }
  }

  private void GameInput_OnInteratAction(object sender, EventArgs e) {
    if (!KitchenGameManager.Instance.IsGamePlaying()) {
      return;
    }

    if (selectedCounter != null) {
      selectedCounter.Interact(this);
    }
  }

  private void Update() {
    if (!IsOwner) {
      // script bize ait deðilse çýk buradan
      return;
    }

    // sadece local player için çalýþacak kýsým
    handleMovement();
    handleInteractions();
  }

  private void handleInteractions() {
    Vector2 inputVector = gameInputInstance.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    // hareketi kestiðimizde interact devam etmesi için sonuncu yönü kaydet.
    if (moveDir != Vector3.zero) {
      lastInteractDir = moveDir;
    }

    float interactDistance = 2f;
    if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
      if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
        if (baseCounter != selectedCounter) {
          // buraya geldiysek bir kutuya deymekteyiz.
          SetSelectedCounter(baseCounter);
        }
      } else {
        // clear counter scripti çarptýðýmýz objede yok ise
        SetSelectedCounter(null);
      }
    } else {
      // önümüzde hiç bir kutu yok açýk arazi
      SetSelectedCounter(null);
    }
  }

  private void SetSelectedCounter(BaseCounter selectedCounter) {
    this.selectedCounter = selectedCounter;
    OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
      selectedCounter = selectedCounter
    });
  }

  private void handleMovement() {
    Vector2 inputVector = gameInputInstance.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    float moveDistance = Time.deltaTime * moveSpeed;
    float playerRadius = .6f;
    float playerHeight = 2f;
    var canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, collisionsLayerMask);

    // X veya Z de engel Var
    if (!canMove) {
      Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
      canMove = (moveDir.x < -.5f || moveDir.x > .5f) &&
        !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);

      // X eksenin de engel yok Z ekseninde Var
      if (canMove) {
        moveDir = moveDirX;

        // X ekseninde engel var Z ekseninde yok
      } else {
        Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
        canMove = (moveDir.z < -.5f || moveDir.z > .5f) &&
          !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);

        // X engel var Z yok
        if (canMove) {
          moveDir = moveDirZ;
        } else {
          // 2 tarafta engelli
          //moveDir = Vector3.zero;
        }
      }
    }

    // X ve Z eksenlerinde engel yok
    if (canMove) {
      transform.position += moveDir * moveDistance;
    }

    isWalking = moveDir != Vector3.zero;

    float rotationSpeed = 10f;
    transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
  }

  public bool IsWalking() {
    return isWalking;
  }

  /// oyuncunun spawn noktasýný dön
  public Transform GetKitchenObjectFollowTransform() {
    return kitchenObjectHoldPoint;
  }

  /// oyuncunun üzerindeki malzemeyi deðiþtir
  public void SetKitchenObject(KitchenObject kitchenObject) {
    this.kitchenObject = kitchenObject;
    if (kitchenObject != null) {
      OnPickedSomething?.Invoke(this, EventArgs.Empty);
      OnAnyPickSomething?.Invoke(this, EventArgs.Empty);
    }
  }

  /// oyuncunun üzerindeki malzemeyi dön
  public KitchenObject GetKitchenObject() {
    return kitchenObject;
  }

  /// oyuncunun üzerini temizle
  public void ClearKitchenObject() {
    kitchenObject = null;
  }

  /// oyuncunun üzerinde bir malzeme var mý?
  public bool HasKitchenObject() {
    return kitchenObject != null;
  }

  public NetworkObject GetNetworkObject() {
    return NetworkObject;
  }
}