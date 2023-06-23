using System;

public interface IHasProgress {

  /// progres de�i�ti�inde �al��acak event
  public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

  public class OnProgressChangedEventArgs : EventArgs {
    public float progressNormalized;
  }
}