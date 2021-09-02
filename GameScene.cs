//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameScene : MonoBehaviourPunCallbacks{
  string cname;
  public static Vector3 stageCenter = new Vector3(-898.5f, 233.27f, 603.7f);//対戦場所の中心座標

  static int rockNum = 9;//岩の数
  public static int GetRockNum(){return rockNum;}

  //各オブジェクトの座標
  Vector3[] rockPosition = new Vector3[rockNum];
  Vector3[] characterPosition = new Vector3[rockNum];//プレイヤーは岩の後ろに隠れるため、プレイヤーの立ち位置も岩と同数
  Vector3[] targetPosition = new Vector3[rockNum];//円型に並んでいるので岩の数と岩の間の数は同数
  Vector3 cameraHeight = new Vector3(0, 10, 0);
  //int playerPosition = 0;
  int[] playerPosition = new int[5];
  GameObject[] player = new GameObject[5];
  public GameObject mainCamera;
  //int playerCameraDistance = 10;

  byte myNumber;
  string[] playersName = new string[5];

  byte[] locate = new byte[5];
  byte[] attack = new byte[5];


  // Start is called before the first frame update
  void Start(){
    PhotonNetwork.IsMessageQueueRunning = true;
    cname = SelectScene.GetCharacterName();//SelectSceneで選択したキャラクターをcnameに設定
    CalculatePosition();//岩やプレイヤーの位置の計算
    for(int i = 0; i < rockNum; i++){
      PhotonNetwork.Instantiate("rock", rockPosition[i], Quaternion.identity);//生成
    }

    //移動と攻撃の初期化
    for(int i = 0; i < 5; i++){
      locate[i] = 0;
      attack[i] = 0;
    }

    var localPlayer = PhotonNetwork.LocalPlayer;//自分のデータをPhotonから取得
    myNumber = (byte)localPlayer.ActorNumber;//自分のIDを取得

    var players = PhotonNetwork.PlayerList;//自分を含んだ全プレイヤーのデータの配列
    for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++){
      Debug.Log($"pn:{players[i].ActorNumber}");
      playersName[players[i].ActorNumber/*0番目のplayersのActorNumberは1になる*/] = players[i].NickName;
    }

    //選択したキャラクターを生成
    /*GameObject selectedCharacter = Resources.Load<GameObject>(cname);
    player = Instantiate(selectedCharacter, characterPosition[playerPosition[myNumber]], Quaternion.identity);*/
    for(int i = 1; i <= PhotonNetwork.CurrentRoom.PlayerCount; i++){
      GameObject selectedCharacter = Resources.Load<GameObject>(playersName[i]);
      playerPosition[i] = MatchingScene.playerPosition[i];// 0 ~ rockNum-1 までの乱数で指定したい
      player[i] = Instantiate(selectedCharacter, characterPosition[playerPosition[i]], Quaternion.identity);
    }

    //カメラをキャラクターの外側のちょっと上に中心を見るように設置
    mainCamera.transform.position = stageCenter + 1.3f*(characterPosition[playerPosition[myNumber]]-stageCenter) + cameraHeight;
    mainCamera.transform.LookAt(stageCenter);

    Debug.Log("your id is "+myNumber);
  }

  // Update is called once per frame
  void Update(){
    for(int n = 1; n <= PhotonNetwork.CurrentRoom.PlayerCount; n++){
      switch(action[n]){
      case "moveright":
        MoveRight(n/*ここの引数は動かしたいプレイヤーのIDなんだけどどうやって全員動かそうかね？*/);
        break;

      case "moveleft":
        MoveLeft(n);
        break;

      case "shot":
        Shot(n);
        break;
      }
    }
    switch(action[myNumber]){
      case "aim":
        //中心から、一つ隣の岩と今いる岩までのベクトル和の半分　つまり、二つの岩の真ん中になる。
        Vector3 viewPosition = (characterPosition[playerPosition[myNumber]] + characterPosition[(playerPosition[myNumber] +1)%rockNum] - 2*stageCenter) * 0.4f;
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, stageCenter + viewPosition + cameraHeight, 100.0f * Time.deltaTime);
        mainCamera.transform.LookAt(targetPosition[(playerPosition[myNumber]+4)%rockNum] + cameraHeight);//反対側の右寄りの岩にカメラを向けておく
        break;

      case "back":
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, stageCenter + 1.3f*(characterPosition[playerPosition[myNumber]]-stageCenter) + cameraHeight, 100.0f * Time.deltaTime);
        mainCamera.transform.LookAt(stageCenter);
        break;

    }

  }

  void MoveRight(int playerID){
    //プレイヤーの位置の移動（カメラも一緒に移動させる）
    player[playerID].transform.position = Vector3.MoveTowards(player[playerID].transform.position, characterPosition[playerPosition[playerID]], 100.0f * Time.deltaTime);
    if(playerID == myNumber){//自分のキャラのときはカメラも動かす
      mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, stageCenter + 1.3f*(characterPosition[playerPosition[myNumber]]-stageCenter) + cameraHeight, 100.0f * Time.deltaTime);
      mainCamera.transform.LookAt(stageCenter);//ステージの中心にカメラを向けておく
    }
  }

  void MoveLeft(int playerID){
    //プレイヤーの位置の移動（カメラも一緒に移動させる）
    player[playerID].transform.position = Vector3.MoveTowards(player[playerID].transform.position, characterPosition[playerPosition[playerID]], 100.0f * Time.deltaTime);
    if(playerID == myNumber){//自分のキャラのときはカメラも動かす
      mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, stageCenter + 1.3f*(characterPosition[playerPosition[myNumber]]-stageCenter) + cameraHeight, 100.0f * Time.deltaTime);
      mainCamera.transform.LookAt(stageCenter);//ステージの中心にカメラを向けておく
    }
  }

  void Shot(int playerID){

  }

  //ターン開始時の処理
  void OnStartTurn(){//initialize and decide action
    for(int i = 0; i < 5; i++){
      locate[i] = 0;
      attack[i] = 0;
    }
    endSelectAction=0;
    InitializeAction();
  }

  //ターン中の処理
  void OnTurn(){//calculate result
    //updatecustompropaties
    UpdateCustomPropaties();

    //wait until all player propaties update

  }

  //ターン終了時の処理
  void OnEndTurn(){
    WhoKillWho();//calculate
    for(int n = 1; n <= PhotonNetwork.CurrentRoom.PlayerCount; n++){
      for(int i = 0; i < 16; i++){
        if(result[i].victim == n){
          DeadAnimation(n);
          goto End;
        }
        if(result[i].killer == n){
          KillAnimation(n);
          goto End;
        }
      }
      if(locate[n] <= 88){//移動
        if(locate[n]%11 == 0){
          action[n] = "moveleft";
          //playerPosition[n] = (playerPosition[n] -1 +rockNum)%rockNum;
        }else{
          action[n] = "moveright";
          //playerPosition[n] = (playerPosition[n] +1)%rockNum;
        }
      }else if(attack[n] == 9){
        action[n] = "wait";
      }else{
        action[n] = "shot";
      }
      End: Debug.Log("hello");
    }
    //run animation

    //this function call when user's action is decided.
    //so this should submit user's action.
    OnStartTurn();
  }

  void DeadAnimation(int playerID){}
  void KillAnimation(int playerID){}

  enum Site{rock, target}

  class Result{
    public int victim, killer, site, position, isKill;
    public void InputResult(int v, int k, int s, int p, int isK){victim = v; killer = k; site = s; position = p; isKill = isK;}
  };

  Result[] result = new Result[16];

  void InitializeResult(){
    for(int i = 0; i < 16; i++){
      result[i] = new Result();
    }
  }

  void WhoKillWho(){
    Debug.Log("WhoKillWho-----");
    InitializeResult();
    int i = 0;
    for(int pn = 1; pn <= 4; pn++){
      for(int qn = 1; qn <= 4; qn++){
        if(pn!=qn){
          if((locate[pn]-attack[pn])%10 == 0 && (locate[qn]-attack[pn])%10 == 0){
            if((locate[qn]-attack[pn])==90){
              //pn kill qn by knife
              result[i].InputResult(qn, pn, (int)Site.rock, attack[pn], 1);
              Debug.Log(pn+" kill "+qn+" with knife.");
            }else{
              //pn and qn both not death
              result[i].InputResult(qn, pn, (int)Site.rock, attack[pn], 0);
              Debug.Log(pn+" and "+qn+" meet in same space.");
            }
          }else if((locate[pn]-90) >= 0 && (locate[qn]-attack[pn]*10) >= 0 && (locate[qn]-90) < 0){
            //pn kill qn by gun
            result[i].InputResult(qn, pn, (int)Site.target, attack[pn], 1);
            Debug.Log(pn+" kill "+qn+" with gun.");
          }
          i++;
        }
      }
    }
    Debug.Log("WhoKillWho?");
    Debug.Log(result);
  }

  //カスタムプロパティ関連
  void UpdateCustomPropaties(){
    Debug.Log($"Update Custom Propaties / locate:{locate[myNumber]} / attack:{attack[myNumber]}");
    Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
    hashtable["locate"] = locate[myNumber];
    hashtable["attack"] = attack[myNumber];
    PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    hashtable.Clear();
  }//最初のプレイヤーのidは1のようです。

  int endSelectAction = 0;
  public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){//カスタムプロパティが変更されるたびに呼び出されるらしい
    Debug.Log("Custom Properties Updated");

    locate[targetPlayer.ActorNumber] = (targetPlayer.CustomProperties["locate"] is byte value1) ? (byte)value1 : (byte)9;
    attack[targetPlayer.ActorNumber] = (targetPlayer.CustomProperties["attack"] is byte value2) ? (byte)value2 : (byte)9;
    //Debug.Log($"get locale : {PhotonNetwork.CurrentRoom.CustomProperties["locate"]} is converted to {locate[targetPlayer.ActorNumber]}");
    //Debug.Log("playerNumber:"+targetPlayer.ActorNumber+"\tlocale:"+locate[targetPlayer.ActorNumber]+"\tattack:"+attack[targetPlayer.ActorNumber]);

    //全員のデータが集まるまで次のターンを開始しない
    if(locate[targetPlayer.ActorNumber]!=9){
      endSelectAction++;
    }
    if(endSelectAction>=4){
      OnEndTurn();
      endSelectAction=0;
    }
    Debug.Log("endSelectAction:"+endSelectAction);
  }

  void CalculatePosition(){
    int stageRadius = 30;//対戦場所の中心座標から岩までの半径

    //岩の座標
    float theta = 0;
    for(int i = 0; i < rockNum; i++){
      rockPosition[i] = new Vector3(stageCenter.x + stageRadius*Mathf.Cos(theta * Mathf.Deg2Rad), stageCenter.y, stageCenter.z + stageRadius*Mathf.Sin(theta * Mathf.Deg2Rad));
      theta += 360.0f/rockNum;
    }

    //プレイヤーの待機場所の座標
    int characterRadius = stageRadius + 10;
    theta = 0;
    for(int i=0; i < rockNum; i++){
      characterPosition[i] = new Vector3(stageCenter.x + characterRadius*Mathf.Cos(theta * Mathf.Deg2Rad), stageCenter.y, stageCenter.z + characterRadius*Mathf.Sin(theta * Mathf.Deg2Rad));
      theta += 360.0f/rockNum;
    }

    //射撃場所の座標
    theta = (360.0f/rockNum)/2;//最初の角を半分ずらす
    for(int i = 0; i < rockNum; i++){
      targetPosition[i] = new Vector3(stageCenter.x + stageRadius*Mathf.Cos(theta * Mathf.Deg2Rad), stageCenter.y+5, stageCenter.z + stageRadius*Mathf.Sin(theta * Mathf.Deg2Rad));
      theta += 360.0f/rockNum;
    }
  }

  string[] action = new string[5];

  void InitializeAction(){
    for(int i = 0; i <= 4; i++){
      action[i] = "none";
    }
  }

  public void OnAimButton(){
    action[myNumber] = "aim";
  }

  public void OnShotLeftButton(){
    action[myNumber] = "shotleft";
    locate[myNumber] = (byte)(90 + playerPosition[myNumber]);
    attack[myNumber] = (byte)((playerPosition[myNumber] + 5)%rockNum);
    OnTurn();
  }

  public void OnShotCenterButton(){
    action[myNumber] = "shotcenter";
    locate[myNumber] = (byte)(90 + playerPosition[myNumber]);
    attack[myNumber] = (byte)((playerPosition[myNumber] + 4)%rockNum);
    OnTurn();
  }

  public void OnShotRightButton(){
    action[myNumber] = "shotright";
    locate[myNumber] = (byte)(90 + playerPosition[myNumber]);
    attack[myNumber] = (byte)((playerPosition[myNumber] + 3)%rockNum);
    OnTurn();
  }

  // public void OnShotLeftButton(){
  //   action = "shotleft";
  // }

  public void OnRightButton(){
    action[myNumber] = "moveright";
    int currentPosition = playerPosition[myNumber];
    playerPosition[myNumber] = (playerPosition[myNumber] +1)%rockNum;
    locate[myNumber] = (byte)(10*currentPosition + playerPosition[myNumber]);
    attack[myNumber] = (byte)(playerPosition[myNumber]);
    //Debug.Log("playerPosition[myNumber]:"+playerPosition[myNumber]);
    OnTurn();
    //Vector3.MoveTowards(characterPosition[currentPosition], characterPosition[playerPosition[myNumber]], 1.0f * Time.deltaTime);
  }

  public void OnLeftButton(){
    action[myNumber] = "moveleft";
    int currentPosition = playerPosition[myNumber];
    playerPosition[myNumber] = (playerPosition[myNumber] -1 +rockNum)%rockNum;
    locate[myNumber] = (byte)(10*playerPosition[myNumber] + playerPosition[myNumber]);
    attack[myNumber] = (byte)(playerPosition[myNumber]);
    //Debug.Log("playerPosition[myNumber]:"+playerPosition[myNumber]);
    OnTurn();
    //Vector3.MoveTowards(characterPosition[currentPosition], characterPosition[playerPosition[myNumber]], 1.0f * Time.deltaTime);
  }

  public void OnBackButton(){
    action[myNumber] = "back";
  }

  public void OnWaitButton(){
    action[myNumber] = "wait";
    locate[myNumber] = (byte)(90 + playerPosition[myNumber]);
    attack[myNumber] = (byte)(9);
    OnTurn();
  }

}
