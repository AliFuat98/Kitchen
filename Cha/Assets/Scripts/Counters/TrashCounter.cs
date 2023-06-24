using System;

public class TrashCounter : BaseCounter {

  /// ses i�in event
  public static event EventHandler OnAnyObjectTrashed;

  public static new void ResetStaticData() {
    OnAnyObjectTrashed = null;
  }

  public override void Interact(Player player) {
    if (player.HasKitchenObject()) {
      // oyuncunun elinde malzeme var

      // ��pe at
      player.GetKitchenObject().DestroyItelf();
      OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    } else {
      // oyuncunun elinde malzeme yok
    }
  }
}