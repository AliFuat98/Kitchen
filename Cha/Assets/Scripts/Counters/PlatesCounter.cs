using System;
using UnityEngine;

public class PlatesCounter : BaseCounter {

  public event EventHandler<OnPlateSpawnEventArgs> OnPlateSpawn;

  public event EventHandler<OnPlateSpawnEventArgs> OnPlateRemoved;

  public class OnPlateSpawnEventArgs : EventArgs {
    public int platesSpawnedAmount;
  }

  /// bu kutunun üzerinde duracak malzeme
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// 4 saniye de bir spwan edilecek tabak
  private readonly float spawnPlatesTimerMax = 4f;

  private float spawnPlatesTimer;

  private readonly int platesSpawnedAmountMax = 4;
  private int platesSpawnedAmount;

  private void Update() {
    spawnPlatesTimer += Time.deltaTime;
    if (spawnPlatesTimer >= spawnPlatesTimerMax) {
      // yeni tabak çýkmasý için süre geldi

      // süreyi sýfýrla
      spawnPlatesTimer = 0f;

      if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax) {
        // yeni tabak çýkabilecek kadar yer var

        // tabak görseli göster
        OnPlateSpawn?.Invoke(this, new OnPlateSpawnEventArgs {
          platesSpawnedAmount = platesSpawnedAmount,
        });
        platesSpawnedAmount++;
      }
    }
  }

  public override void Interact(Player player) {
    if (!player.HasKitchenObject()) {
      // oyuncunun eli boþ

      if (platesSpawnedAmount > 0) {
        // kutunun üzerinde tabak var

        //  oyuncuya tabak ver
        KitchenObject.SpwanKitchenObject(kitchenObjectSO, player);

        // bir adet tabak görselini sil
        platesSpawnedAmount--;
        OnPlateRemoved?.Invoke(this, new OnPlateSpawnEventArgs {
          platesSpawnedAmount = platesSpawnedAmount
        });
      }
    } else {
      // oyuncunun eli dolu

      //---
    }
  }
}