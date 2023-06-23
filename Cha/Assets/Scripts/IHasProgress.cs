using System;

public interface IHasProgress {

  /// progres deðiþtiðinde çalýþacak event
  public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

  public class OnProgressChangedEventArgs : EventArgs {
    public float progressNormalized;
  }
}