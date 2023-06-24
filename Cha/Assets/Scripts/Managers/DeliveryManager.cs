using System;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour {

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

  /// spwan edilebilecek sipari� listesi
  [SerializeField] private RecipeListSO recipeSOList;

  /// bekleyen sipari�ler
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
      // yeni bir sipari� spawn vakti

      // s�reyi ba�a al
      spawnRecipeTimer = spawnRecipeTimerMax;

      if (waitingRecipeSOList.Count < waitingRecipeMax) {
        // yeni sipari� i�in yer var

        // listeden rastgele bir sipari� se�
        RecipeSO waitingRecipeSO = recipeSOList.recipeSOList[UnityEngine.Random.Range(0, recipeSOList.recipeSOList.Count)];

        // listeye ekle
        waitingRecipeSOList.Add(waitingRecipeSO);

        // event ba�lat
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
      }
    }
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

          // bekleyen sipari� listesinden kald�r
          waitingRecipeSOList.RemoveAt(i);

          // eventleri ba�lat
          OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
          OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
          successfullDeliverAmount++;

          return;
        } else {
          //  gezdi�imiz s�radaki sipari� e�le�me sa�lamad� => sonrakilere bakmaya devam et
        }
      } else {
        // gezdi�imiz s�radaki sipari�te malzeme say�s� tutmuyor  => sonrakilere bakmaya devam et
      }
    }

    // tabaktaki malzeme ile e�le�en bir sipari� yok => sipari� yanl�� haz�rlanm��
    OnRecipeFailed?.Invoke(this, EventArgs.Empty);
  }

  public List<RecipeSO> GetWaitingRecipeSOList() {
    return waitingRecipeSOList;
  }

  public int GetSuccessfullDeliverAmount() {
    return successfullDeliverAmount;
  }
}