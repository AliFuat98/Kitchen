using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {

  /// Singleton pattern
  public static Player Instance { get; private set; }

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

  /// HIZ
  [SerializeField] private float moveSpeed = 7f;

  /// NEW INPUT SYSTEM instance
  [SerializeField] private GameInput gameInput;

  /// kutularý raycast ile hit yaparken maskeleme yapmak için
  [SerializeField] private LayerMask countersLayerMask;

  /// malzemeyi nerde spawn edicez = oyuncunun malzemeyi tutma noktasý
  [SerializeField] private Transform kitchenObjectHoldPoint;

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

  private void Awake() {
    // Singleton
    if (Instance != null) {
      Debug.LogError("there is more than one player");
      return;
    }
    Instance = this;
  }

  private void Start() {
    // interaction tuþuna (E and F) tuþuna basýnca çalýþan event'e sub oluyoruz

    // E basýnca GameInput_OnInteratAction çalýþacak
    gameInput.OnInteractAction += GameInput_OnInteratAction;

    // F basýnca GameInput_OnInteractAlternateAction çalýþacak
    gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
  }

  private void GameInput_OnInteractAlternateAction(object sender, EventArgs e) {
    if (selectedCounter != null) {
      selectedCounter.InteractAlternate(this);
    }
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
    Vector2 inputVector = gameInput.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    float moveDistance = Time.deltaTime * moveSpeed;
    float playerRadius = .6f;
    float playerHeight = 2f;
    var canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

    // X veya Z de engel Var
    if (!canMove) {
      Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
      canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

      // X eksenin de engel yok Z ekseninde Var
      if (canMove) {
        moveDir = moveDirX;

        // X ekseninde engel var Z ekseninde yok
      } else {
        Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
        canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

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
}