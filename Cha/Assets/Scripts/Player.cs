using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent {

  /// Singleton pattern
  public static Player LocalInstance { get; private set; }

  /// <summary>
  /// bir oyuncu sahnede spawn oldu�unda �al��acak event
  /// static yapmam�z�n sebebide eventin class'�n kendine ait olmas�
  /// spesifik bir player'a de�il
  /// </summary>
  public static event EventHandler OnAnyPlayerSpawned;

  public static event EventHandler OnAnyPickSomething;

  public static void ResetStaticData() {
    OnAnyPlayerSpawned = null;
    OnAnyPickSomething = null;
  }

  /// <summary>
  /// oyuncu se�ili kutuyu de�i�tirdi�inde �al��acak
  /// burada Publisher'�m�z Player
  /// subs'lar ise kutular olucak. Kutular bu eventi dinlemede bekliyor.
  /// De�i�im oldu�unda t�m sub olan kutular atad�klar� fonksiyonu execute edicek (hide or show)
  /// hangi kutuyu se�ti�imizi sub olan kutular�n anlamas� i�inde arg�man olarak yolluyoruz
  /// </summary>
  public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

  public class OnSelectedCounterChangedEventArgs : EventArgs {
    public BaseCounter selectedCounter;
  }

  /// ses i�in event
  public event EventHandler OnPickedSomething;

  /// HIZ
  [SerializeField] private float moveSpeed = 7f;

  /// kutular� raycast ile hit yaparken maskeleme yapmak i�in
  [SerializeField] private LayerMask countersLayerMask;

  /// �arp��malar i�in maske
  [SerializeField] private LayerMask collisionsLayerMask;

  /// malzemeyi nerde spawn edicez = oyuncunun malzemeyi tutma noktas�
  [SerializeField] private Transform kitchenObjectHoldPoint;

  /// oyuncular�n spwan olucaklar� noktalar
  [SerializeField] private List<Vector3> spawnPositionList;

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
  private BaseCounter selectedCounter;

  GameInput gameInputInstance;

  public override void OnNetworkSpawn() {
    if (IsOwner) {
      // �uan local player'day�z

      LocalInstance = this;
    }
    transform.position = spawnPositionList[(int)OwnerClientId];
    OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
  }

  private void Start() {
    gameInputInstance = GameInput.Instance;

    // interaction tu�una (E and F) tu�una bas�nca �al��an event'e sub oluyoruz

    // E bas�nca GameInput_OnInteratAction �al��acak
    gameInputInstance.OnInteractAction += GameInput_OnInteratAction;

    // F bas�nca GameInput_OnInteractAlternateAction �al��acak
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
      // script bize ait de�ilse ��k buradan
      return;
    }

    // sadece local player i�in �al��acak k�s�m
    handleMovement();
    handleInteractions();
  }

  private void handleInteractions() {
    Vector2 inputVector = gameInputInstance.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    // hareketi kesti�imizde interact devam etmesi i�in sonuncu y�n� kaydet.
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
        // clear counter scripti �arpt���m�z objede yok ise
        SetSelectedCounter(null);
      }
    } else {
      // �n�m�zde hi� bir kutu yok a��k arazi
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

  /// oyuncunun spawn noktas�n� d�n
  public Transform GetKitchenObjectFollowTransform() {
    return kitchenObjectHoldPoint;
  }

  /// oyuncunun �zerindeki malzemeyi de�i�tir
  public void SetKitchenObject(KitchenObject kitchenObject) {
    this.kitchenObject = kitchenObject;
    if (kitchenObject != null) {
      OnPickedSomething?.Invoke(this, EventArgs.Empty);
      OnAnyPickSomething?.Invoke(this, EventArgs.Empty);
    }
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

  public NetworkObject GetNetworkObject() {
    return NetworkObject;
  }
}