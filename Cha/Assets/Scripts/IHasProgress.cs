using System;

public interface IHasProgress {

  /// progres değiştiğinde çalışacak event
  public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

  public class OnProgressChangedEventArgs : EventArgs {
    public float progressNormalized;
  }
}