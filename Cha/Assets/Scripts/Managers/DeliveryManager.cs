using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour {

  /// singleton
  public static DeliveryManager Instance { get; private set; }

  /// yeni sipariþ geldiðinde çalýþacak event
  public event EventHandler OnRecipeSpawned;

  /// sipariþ tamamlanýnca çalýþacak event
  public event EventHandler OnRecipeCompleted;

  /// sipariþ doðruysa çalýþacak event
  public event EventHandler OnRecipeSuccess;

  /// sipariþ yanluþ ise çalýþacak evet
  public event EventHandler OnRecipeFailed;

  /// verilen sipariþ artýnca çalýþacak event
  public event EventHandler OnDeliveryAmountChanged;

  /// spwan edilebilecek sipariþ listesi
  [SerializeField] private RecipeListSO recipeSOList;

  /// bekleyen sipariþler
  private List<RecipeSO> waitingRecipeSOList;

  private float spawnRecipeTimer = 4;
  private float spawnRecipeTimerMax = 4f;

  private int waitingRecipeMax = 4;

  private int xSuccessfullDeliverAmount;

  private int SuccessfullDeliverAmount {
    get { return xSuccessfullDeliverAmount; }
    set {
      xSuccessfullDeliverAmount = value;
      OnDeliveryAmountChanged?.Invoke(this, EventArgs.Empty);
    }
  }

  private void Awake() {
    if (Instance != null) {
      Debug.Log("birden fazla delivery manager");
    }
    Instance = this;

    waitingRecipeSOList = new List<RecipeSO>();

    spawnRecipeTimer = spawnRecipeTimerMax;
  }

  private void Update() {
    if (!IsServer) {
      return;
    }

    spawnRecipeTimer -= Time.deltaTime;
    if (spawnRecipeTimer < 0f) {
      // yeni bir sipariþ spawn vakti

      // süreyi baþa al
      spawnRecipeTimer = spawnRecipeTimerMax;

      if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax) {
        // yeni sipariþ için yer var

        // tüm client'lerde yeni bir sipariþ oluþtur
        SpawnNewWaitingRecipeClientRpc(UnityEngine.Random.Range(0, recipeSOList.recipeSOList.Count));
      }
    }
  }

  [ClientRpc]
  private void SpawnNewWaitingRecipeClientRpc(int recipeSOListRandomIndex) {
    // listeden rastgele bir sipariþ seç
    // rastgele sayý server'dan gelicek
    RecipeSO waitingRecipeSO = recipeSOList.recipeSOList[recipeSOListRandomIndex];

    // listeye ekle
    waitingRecipeSOList.Add(waitingRecipeSO);

    // event baþlat
    OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
  }

  public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
    // tabaðýn üzerindeki malzemeleri tutan liste
    var plateKitchenObjectSOList = plateKitchenObject.GetKitchenObjectSOList();

    // bekleyen bir sipariþle tabak uyuþuyor mu hepsine tek tek bakýyoruz
    for (int i = 0; i < waitingRecipeSOList.Count; i++) {
      RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

      if (waitingRecipeSO.KitchenObjectSOList.Count == plateKitchenObjectSOList.Count) {
        // ayný sayýda malzeme var

        bool recipeMatch = true;
        foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.KitchenObjectSOList) {
          // sipariþteki ürünlere bakýyoruz

          if (!plateKitchenObjectSOList.Contains(recipeKitchenObjectSO)) {
            // tabakta eksik var
            recipeMatch = false;
            break;
          } else {
            // tabakta var sipariþteki diðer ürünlere bakabiliriz

            // ---
          }
        }

        if (recipeMatch) {
          // tabaktaki malzeme ile eþleþen bir sipariþ var => sipariþ doðru hazýrlanmýþ

          DeliverCorrectRecipeServerRpc(i);

          return;
        } else {
          //  gezdiðimiz sýradaki sipariþ eþleþme saðlamadý => sonrakilere bakmaya devam et
        }
      } else {
        // gezdiðimiz sýradaki sipariþte malzeme sayýsý tutmuyor  => sonrakilere bakmaya devam et
      }
    }

    // tabaktaki malzeme ile eþleþen bir sipariþ yok => sipariþ yanlýþ hazýrlanmýþ
    DeliverInCorrectRecipeServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  private void DeliverCorrectRecipeServerRpc(int recipeIndex) {
    RecipeSO recipeSO = waitingRecipeSOList.ElementAt(recipeIndex);
    if (recipeSO.KitchenObjectSOList.Count == 5) {
      // mega burger geldi
      KitchenGameManager.Instance.gamePlayingTimer.Value += 2;
    } else {
      // baþka bir malzeme geldi
      KitchenGameManager.Instance.gamePlayingTimer.Value += 1;
    }
    DeliverCorrectRecipeClientRpc(recipeIndex);
  }

  [ClientRpc]
  private void DeliverCorrectRecipeClientRpc(int recipeIndex) {
    // bekleyen sipariþ listesinden kaldýr
    waitingRecipeSOList.RemoveAt(recipeIndex);

    // eventleri baþlat
    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    SuccessfullDeliverAmount++;
  }

  [ServerRpc(RequireOwnership = false)]
  private void DeliverInCorrectRecipeServerRpc() {
    DeliverInCorrectRecipeClientRpc();
  }

  [ClientRpc]
  private void DeliverInCorrectRecipeClientRpc() {
    OnRecipeFailed?.Invoke(this, EventArgs.Empty);
  }

  public List<RecipeSO> GetWaitingRecipeSOList() {
    return waitingRecipeSOList;
  }

  public int GetSuccessfullDeliverAmount() {
    return SuccessfullDeliverAmount;
  }
}