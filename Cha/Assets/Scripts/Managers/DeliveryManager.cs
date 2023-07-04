using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour {

  /// singleton
  public static DeliveryManager Instance { get; private set; }

  /// yeni sipari� geldi�inde �al��acak event
  public event EventHandler OnRecipeSpawned;

  /// sipari� tamamlan�nca �al��acak event
  public event EventHandler OnRecipeCompleted;

  /// sipari� do�ruysa �al��acak event
  public event EventHandler OnRecipeSuccess;

  /// sipari� yanlu� ise �al��acak evet
  public event EventHandler OnRecipeFailed;

  /// verilen sipari� art�nca �al��acak event
  public event EventHandler OnDeliveryAmountChanged;

  /// spwan edilebilecek sipari� listesi
  [SerializeField] private RecipeListSO recipeSOList;

  /// bekleyen sipari�ler
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
      // yeni bir sipari� spawn vakti

      // s�reyi ba�a al
      spawnRecipeTimer = spawnRecipeTimerMax;

      if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax) {
        // yeni sipari� i�in yer var

        // t�m client'lerde yeni bir sipari� olu�tur
        SpawnNewWaitingRecipeClientRpc(UnityEngine.Random.Range(0, recipeSOList.recipeSOList.Count));
      }
    }
  }

  [ClientRpc]
  private void SpawnNewWaitingRecipeClientRpc(int recipeSOListRandomIndex) {
    // listeden rastgele bir sipari� se�
    // rastgele say� server'dan gelicek
    RecipeSO waitingRecipeSO = recipeSOList.recipeSOList[recipeSOListRandomIndex];

    // listeye ekle
    waitingRecipeSOList.Add(waitingRecipeSO);

    // event ba�lat
    OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
  }

  public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
    // taba��n �zerindeki malzemeleri tutan liste
    var plateKitchenObjectSOList = plateKitchenObject.GetKitchenObjectSOList();

    // bekleyen bir sipari�le tabak uyu�uyor mu hepsine tek tek bak�yoruz
    for (int i = 0; i < waitingRecipeSOList.Count; i++) {
      RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

      if (waitingRecipeSO.KitchenObjectSOList.Count == plateKitchenObjectSOList.Count) {
        // ayn� say�da malzeme var

        bool recipeMatch = true;
        foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.KitchenObjectSOList) {
          // sipari�teki �r�nlere bak�yoruz

          if (!plateKitchenObjectSOList.Contains(recipeKitchenObjectSO)) {
            // tabakta eksik var
            recipeMatch = false;
            break;
          } else {
            // tabakta var sipari�teki di�er �r�nlere bakabiliriz

            // ---
          }
        }

        if (recipeMatch) {
          // tabaktaki malzeme ile e�le�en bir sipari� var => sipari� do�ru haz�rlanm��

          DeliverCorrectRecipeServerRpc(i);

          return;
        } else {
          //  gezdi�imiz s�radaki sipari� e�le�me sa�lamad� => sonrakilere bakmaya devam et
        }
      } else {
        // gezdi�imiz s�radaki sipari�te malzeme say�s� tutmuyor  => sonrakilere bakmaya devam et
      }
    }

    // tabaktaki malzeme ile e�le�en bir sipari� yok => sipari� yanl�� haz�rlanm��
    DeliverInCorrectRecipeServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  private void DeliverCorrectRecipeServerRpc(int recipeIndex) {
    RecipeSO recipeSO = waitingRecipeSOList.ElementAt(recipeIndex);
    if (recipeSO.KitchenObjectSOList.Count == 5) {
      // mega burger geldi
      KitchenGameManager.Instance.gamePlayingTimer.Value += 2;
    } else {
      // ba�ka bir malzeme geldi
      KitchenGameManager.Instance.gamePlayingTimer.Value += 1;
    }
    DeliverCorrectRecipeClientRpc(recipeIndex);
  }

  [ClientRpc]
  private void DeliverCorrectRecipeClientRpc(int recipeIndex) {
    // bekleyen sipari� listesinden kald�r
    waitingRecipeSOList.RemoveAt(recipeIndex);

    // eventleri ba�lat
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