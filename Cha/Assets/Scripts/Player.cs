using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {

  /// Singleton pattern
  public static Player Instance { get; private set; }

  /// <summary>
  /// oyuncu se�ili kutuyu de�i�tirdi�inde �al��acak
  /// burada Publisher'�m�z Player
  /// subs'lar ise kutular olucak. Kutular bu eventi dinlemede bekliyor.
  /// De�i�im oldu�unda t�m sub olan kutular atad�klar� fonksiyonu execute edicek (hide or show)
  /// hangi kutuyu se�ti�imizi sub olan kutular�n anlamas� i�inde arg�man olarak yolluyoruz
  /// </summary>
  public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

  public class OnSelectedCounterChangedEventArgs : EventArgs {
    public ClearCounter selectedCounter;
  }

  /// HIZ
  [SerializeField] private float moveSpeed = 7f;

  /// NEW INPUT SYSTEM instance
  [SerializeField] private GameInput gameInput;

  /// kutular� raycast ile hit yaparken maskeleme yapmak i�in
  [SerializeField] private LayerMask countersLayerMask;

  /// malzemeyi nerde spawn edicez = oyuncunun malzemeyi tutma noktas�
  [SerializeField] private Transform kitchenObjectHoldPoint;

  /// oyuncunun �zerindeki malzeme
  private KitchenObject kitchenObject;

  /// Animation
  private bool isWalking;

  /// <summary>
  /// hareket etmeyi b�rakt���m�zda en son interact etti�imiz y�n� kaybetmemek i�in
  /// mesela kutunun �n�ne kadar geldin hareketi kestin son y�n tekrar s�f�rlan�yor s�f�rlanmadan �nceki de�eri tutar
  /// </summary>
  private Vector3 lastInteractDir;

  /// <summary>
  /// En son se�ili olan kutuyu tutar.
  /// bo� arazide geziyorsak yak�n�m�zda kutu yoksa bu de�er null oluyor
  /// </summary>
  private ClearCounter selectedCounter;

  private void Awake() {
    // Singleton
    if (Instance != null) {
      Debug.LogError("there is more than one player");
      return;
    }
    Instance = this;
  }

  private void Start() {
    // interaction tu�una (E) tu�una bas�nca �al��an event'e sub oluyoruz e bas�ld���nda
    // GameInput_OnInteratAction �al��acak olan fonksiyon
    gameInput.OnInteratAction += GameInput_OnInteratAction;
  }

  private void GameInput_OnInteratAction(object sender, System.EventArgs e) {
    if (selectedCounter != null) {
      selectedCounter.Interact(this);
    }
  }

  private void Update() {
    handleMovement();
    handleInteractions();
  }

  private void handleInteractions() {
    Vector2 inputVector = gameInput.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    // hareketi kesti�imizde interact devam etmesi i�in sonuncu y�n� kaydet.
    if (moveDir != Vector3.zero) {
      lastInteractDir = moveDir;
    }

    float interactDistance = 2f;
    if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
      if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
        if (clearCounter != selectedCounter) {
          // buraya geldiysek bir kutuya deymekteyiz.
          SetSelectedCounter(clearCounter);
        }
      } else {
        // clear counter scripti �arpt���m�z objede yok ise
        SetSelectedCounter(null);
      }
    } else {
      // �n�m�zde hi� bir kutu yok a��k arazi
      SetSelectedCounter(null);
    }
  }

  private void SetSelectedCounter(ClearCounter selectedCounter) {
    this.selectedCounter = selectedCounter;
    OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
      selectedCounter = selectedCounter
    });
  }

  private void handleMovement() {
    Vector2 inputVector = gameInput.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    float moveDistance = Time.deltaTime * moveSpeed;
    float playerRadius = .6f;
    float playerHeight = 2f;
    var canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

    // X veya Z de engel Var
    if (!canMove) {
      Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
      canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

      // X eksenin de engel yok Z ekseninde Var
      if (canMove) {
        moveDir = moveDirX;

        // X ekseninde engel var Z ekseninde yok
      } else {
        Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
        canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

        // X engel var Z yok
        if (canMove) {
          moveDir = moveDirZ;
        } else {
          // 2 tarafta engelli
          moveDir = Vector3.zero;
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

  /// oyuncunun spawn noktas�n� d�n
  public Transform GetKitchenObjectFollowTransform() {
    return kitchenObjectHoldPoint;
  }

  /// oyuncunun �zerindeki malzemeyi de�i�tir
  public void SetKitchenObject(KitchenObject kitchenObject) {
    this.kitchenObject = kitchenObject;
  }

  /// oyuncunun �zerindeki malzemeyi d�n
  public KitchenObject GetKitchenObject() {
    return kitchenObject;
  }

  /// oyuncunun �zerini temizle
  public void ClearKitchenObject() {
    kitchenObject = null;
  }

  /// oyuncunun �zerinde bir malzeme var m�?
  public bool HasKitchenObject() {
    return kitchenObject != null;
  }
}