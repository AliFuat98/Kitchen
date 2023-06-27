using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {

  /// <summary>
  /// bu de�i�ken kutuyu kapsayan en d��taki gameObject'tir
  /// player'daki Player_OnSelectedCounterChanged eventine sub olduktan sonra de�i�en kutu biz miyiz onun kontrol� i�in
  /// </summary>
  [SerializeField] private BaseCounter baseCounter;

  /// <summary>
  /// se�iliysek tipimiz de�i�sin die a� kapa yapaca��mz obje
  /// Selected gameObjesinin alt�ndaki visual
  /// </summary>
  [SerializeField] private GameObject[] visualGameObjectArray;

  private void Start() {
    if (Player.LocalInstance != null) {
      // instance varsa direk event'e subscribe ol
      Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    } else {
      // instance daha olu�mad�ysa

      Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
    }
  }

  private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e) {
    if (Player.LocalInstance != null) {
      // bu event birden fazla kez �al��t��� i�in bir s�r� subscription'�m�z olmamas� i�in
      // bir ade ��kar tak yap�yoruz burda
      Player.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
      Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }
  }

  private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
    if (e.selectedCounter == baseCounter) {
      Show();
    } else {
      Hide();
    }
  }

  private void Show() {
    foreach (var visualGameObject in visualGameObjectArray) {
      visualGameObject.SetActive(true);
    }
  }

  private void Hide() {
    foreach (var visualGameObject in visualGameObjectArray) {
      visualGameObject.SetActive(false);
    }
  }
}