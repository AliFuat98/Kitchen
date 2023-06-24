using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour {

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

  /// spwan edilebilecek sipariþ listesi
  [SerializeField] private RecipeListSO recipeSOList;

  /// bekleyen sipariþler
  private List<RecipeSO> waitingRecipeSOList;

  private float spawnRecipeTimer;
  private float spawnRecipeTimerMax = 4f;

  private int waitingRecipeMax = 4;

  private int successfullDeliverAmount;

  private void Awake() {
    if (Instance != null) {
      Debug.Log("birden fazla delivery manager");
    }
    Instance = this;

    waitingRecipeSOList = new List<RecipeSO>();

    spawnRecipeTimer = spawnRecipeTimerMax;
  }

  private void Update() {
    spawnRecipeTimer -= Time.deltaTime;
    if (spawnRecipeTimer < 0f) {
      // yeni bir sipariþ spawn vakti

      // süreyi baþa al
      spawnRecipeTimer = spawnRecipeTimerMax;

      if (waitingRecipeSOList.Count < waitingRecipeMax) {
        // yeni sipariþ için yer var

        // listeden rastgele bir sipariþ seç
        RecipeSO waitingRecipeSO = recipeSOList.recipeSOList[UnityEngine.Random.Range(0, recipeSOList.recipeSOList.Count)];

        // listeye ekle
        waitingRecipeSOList.Add(waitingRecipeSO);

        // event baþlat
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
      }
    }
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

          // bekleyen sipariþ listesinden kaldýr
          waitingRecipeSOList.RemoveAt(i);

          // eventleri baþlat
          OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
          OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
          successfullDeliverAmount++;

          return;
        } else {
          //  gezdiðimiz sýradaki sipariþ eþleþme saðlamadý => sonrakilere bakmaya devam et
        }
      } else {
        // gezdiðimiz sýradaki sipariþte malzeme sayýsý tutmuyor  => sonrakilere bakmaya devam et
      }
    }

    // tabaktaki malzeme ile eþleþen bir sipariþ yok => sipariþ yanlýþ hazýrlanmýþ
    OnRecipeFailed?.Invoke(this, EventArgs.Empty);
  }

  public List<RecipeSO> GetWaitingRecipeSOList() {
    return waitingRecipeSOList;
  }

  public int GetSuccessfullDeliverAmount() {
    return successfullDeliverAmount;
  }
}