using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {

  /// <summary>
  /// bu de�i�ken kutuyu kapsayan en d��taki gameObject'tir
  /// player'daki Player_OnSelectedCounterChanged eventine sub olduktan sonra de�i�en kutu biz miyiz onun kontrol� i�in
  /// </summary>
  [SerializeField] private ClearCounter clearCounter;

  /// <summary>
  /// se�iliysek tipimiz de�i�sin die a� kapa yapaca��mz obje
  /// Selected gameObjesinin alt�ndaki visual
  /// </summary>
  [SerializeField] private GameObject visualGameObject;

  private void Start() {
    Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
  }

  private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
    if (e.selectedCounter == clearCounter) {
      Show();
    } else {
      Hide();
    }
  }

  private void Show() {
    visualGameObject.SetActive(true);
  }

  private void Hide() {
    visualGameObject.SetActive(false);
  }
}