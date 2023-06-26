using System;
using UnityEngine;

public class PlatesCounter : BaseCounter {

  public event EventHandler<OnPlateSpawnEventArgs> OnPlateSpawn;

  public event EventHandler<OnPlateSpawnEventArgs> OnPlateRemoved;

  public class OnPlateSpawnEventArgs : EventArgs {
    public int platesSpawnedAmount;
  }

  /// bu kutunun �zerinde duracak malzeme
  [SerializeField] private KitchenObjectSO kitchenObjectSO;

  /// 4 saniye de bir spwan edilecek tabak
  private readonly float spawnPlatesTimerMax = 4f;

  private float spawnPlatesTimer;

  private readonly int platesSpawnedAmountMax = 4;
  private int platesSpawnedAmount;

  private void Update() {
    spawnPlatesTimer += Time.deltaTime;
    if (spawnPlatesTimer >= spawnPlatesTimerMax) {
      // yeni tabak ��kmas� i�in s�re geldi

      // s�reyi s�f�rla
      spawnPlatesTimer = 0f;

      if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax) {
        // yeni tabak ��kabilecek kadar yer var

        // tabak g�rseli g�ster
        OnPlateSpawn?.Invoke(this, new OnPlateSpawnEventArgs {
          platesSpawnedAmount = platesSpawnedAmount,
        });
        platesSpawnedAmount++;
      }
    }
  }

  public override void Interact(Player player) {
    if (!player.HasKitchenObject()) {
      // oyuncunun eli bo�

      if (platesSpawnedAmount > 0) {
        // kutunun �zerinde tabak var

        //  oyuncuya tabak ver
        KitchenObject.SpwanKitchenObject(kitchenObjectSO, player);

        // bir adet tabak g�rselini sil
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