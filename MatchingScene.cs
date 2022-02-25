// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

using ExitGames.Client.Photon;

public class MatchingScene : MonoBehaviourPunCallbacks{
  string cname;
  bool isOnRoom = false;
  bool isGetPlayerPosition;
  static int maxPlayer = 4;
  public static int[] playerPosition = new int[maxPlayer];
  public TextMeshProUGUI playerNumberUI;//プレイヤー人数を表示するTextオブジェクト

  // Start is called before the first frame update
  void Start(){
    cname = SelectScene.GetCharacterName();
    Debug.Log(cname);

    PhotonNetwork.NickName = cname;
    playerNumberUI = playerNumberUI.GetComponent<TextMeshProUGUI>();
    PhotonNetwork.ConnectUsingSettings();
  }

  // Update is called once per frame
  void Update(){
    if(isOnRoom){
      playerNumberUI.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
      if(PhotonNetwork.CurrentRoom.IsOpen == false){
        if(PhotonNetwork.LocalPlayer.ActorNumber == 1)SendCustomPropaties();
        //if(isGetPlayerPosition == true)ChangeScene();
      }
    }
  }

  public override void OnConnectedToMaster(){
    //join to random room
    PhotonNetwork.JoinRandomRoom();
  }

  //if there are nothing room to join, run this function
  public override void OnJoinRandomFailed(short returnCode, string message){
    var roomOptions = new RoomOptions();
    roomOptions.MaxPlayers = (byte)maxPlayer;//set max join room as 4

    PhotonNetwork.CreateRoom(null, roomOptions);
  }

  public override void OnJoinedRoom(){
    isOnRoom = true;

    //if room is max, close room
    if(PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers){
      PhotonNetwork.CurrentRoom.IsOpen = false;
    }
  }

  public void ChangeScene(){
    PhotonNetwork.IsMessageQueueRunning = false;
    SceneManager.LoadScene("GameScene");
  }

  //カスタムプロパティ関連
  void SendCustomPropaties(){//送信
    Hashtable randomtable = new ExitGames.Client.Photon.Hashtable();
    for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++){
      playerPosition[i] = Random.Range(0, GameScene.GetRockNum());
      randomtable[$"{i}"] = playerPosition[i];
    }
    PhotonNetwork.LocalPlayer.SetCustomProperties(randomtable);
    randomtable.Clear();
  }//最初のプレイヤーのidは1のようです。

  public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){//カスタムプロパティが変更されるたびに呼び出されるらしい
    if(PhotonNetwork.LocalPlayer.ActorNumber != 1){
      for(int i = 0;i < PhotonNetwork.CurrentRoom.PlayerCount; i++){playerPosition[i] = (targetPlayer.CustomProperties[$"{i}"] is int value1) ? (int)value1 : -1;}
      isGetPlayerPosition = true;
    }
  }
}
