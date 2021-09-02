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
  public static int[] playerPosition = new int[PhotonNetwork.CurrentRoom.MaxPlayers+1];

//  public GameObject playerNumberUI = null;//プレイヤー人数を表示するTextオブジェクト
  public TextMeshProUGUI playerNumberUI = null;//プレイヤー人数を表示するTextオブジェクト

  // Start is called before the first frame update
  void Start(){
    cname = SelectScene.GetCharacterName();
    Debug.Log(cname);

    PhotonNetwork.NickName = cname;
    PhotonNetwork.ConnectUsingSettings();
  }

  // Update is called once per frame
  void Update(){
    if(isOnRoom){
      //Text playerNumber = playerNumberUI.GetComponent<Text>();
      playerNumberUI.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
      if(PhotonNetwork.CurrentRoom.IsOpen == false){
        if(PhotonNetwork.LocalPlayer.ActorNumber == 1)SendCustomPropaties();
        if(isGetPlayerPosition == true)ChangeScene();
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
    roomOptions.MaxPlayers = 4;//set max join room as 4

    PhotonNetwork.CreateRoom(null, roomOptions);
  }

  public override void OnJoinedRoom(){
    //var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
    //PhotonNetwork.Instantiate(cname, position, Quaternion.identity);

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
    for(int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++){
      playerPosition[i] = Random.Range(0, GameScene.GetRockNum());
      randomtable[i] = playerPosition[i];
    }
    PhotonNetwork.LocalPlayer.SetCustomProperties(randomtable);
    randomtable.Clear();
  }//最初のプレイヤーのidは1のようです。

  bool isGetPlayerPosition = (PhotonNetwork.LocalPlayer.ActorNumber == 1) ? true : false;

  public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){//カスタムプロパティが変更されるたびに呼び出されるらしい　∴受信
    if(PhotonNetwork.LocalPlayer.ActorNumber != 1){
      for(int i = 1;i <= PhotonNetwork.CurrentRoom.PlayerCount; i++){playerPosition[i] = (targetPlayer.CustomProperties[i] is int value1) ? (int)value1 : -1;}
      isGetPlayerPosition = true;
    }
  }
}
