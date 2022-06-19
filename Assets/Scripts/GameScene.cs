//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.Collections;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameScene : MonoBehaviourPunCallbacks{
  string sMyName;
  public static Vector3 v3StageCenter = new Vector3(-898.5f, 233.27f, 603.7f);//対戦場所の中心座標

  static int iRockNum = 9;//岩の数
  public static int GetRockNum(){return iRockNum;}

  static int iPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;//プレイヤーの数
  static int iAlivePlayerCount = iPlayerCount;

  //各オブジェクトの座標
         Vector3[] v3RockPos  = new Vector3[iRockNum];
  static Vector3[] v3StandPos = new Vector3[iRockNum];//プレイヤーは岩の後ろに隠れるため、プレイヤーの立ち位置も岩と同数
         Vector3[] v3TrgPos   = new Vector3[iRockNum];//円型に並んでいるので岩の数と岩の間の数は同数
  static Vector3 v3CameraHeight = new Vector3(0, 10, 0);
  static int[] iPlayerPos = new int[iPlayerCount];
  static GameObject[] goPlayer = new GameObject[iPlayerCount];
  public static GameObject mainCamera;

  public static GameObject goShowPlayer(int num){ return goPlayer[num]; }
  public static Vector3    v3ShowPlayerTransPos(int num){ return goPlayer[num].transform.position; }

  public static GameObject goShowCamera(){ return mainCamera; }
  public static Vector3    v3ShowCameraTransPos(){ return mainCamera.transform.position; }

  public static Vector3 v3ShowStandPos(int num){ return v3StandPos[num]; }

  public static Vector3 v3ShowCameraHeight(){ return v3CameraHeight; }
  public static Vector3 v3ShowStageCenter(){ return v3StageCenter; }

  public static void SetPlayerTransPos(int num, Vector3 input){ goPlayer[num].transform.position = input; }
  public static void SetPlayerTransLook(int num, Vector3 input){ goPlayer[num].transform.LookAt(input); }
  public static void SetCameraTransPos(Vector3 input){ mainCamera.transform.position = input; }
  public static void SetCameraTransLook(Vector3 input){ mainCamera.transform.LookAt(input); }
  static float fPlayerSpeed = 10.0f;
  public Text text;
  public GameObject[] CharacterList = new GameObject[4];
  GameObject goSelectCharacter(string sChName){
    switch(sChName){
      case "AvatarFox":    return CharacterList[0]; break;
      case "AvatarHuman":  return CharacterList[1]; break;
      case "AvatarHawk":   return CharacterList[2]; break;
      case "AvatarHelmet": return CharacterList[3]; break;
    }
    return null;
  }

  static byte iMyNumber = (byte)(PhotonNetwork.LocalPlayer.ActorNumber - 1);//自分のIDを取得
  string[] sPlayerName = new string[iPlayerCount];

  static byte[] iLocatePast = new byte[iPlayerCount];//各プレイヤーの元の位置
  static byte[] iLocate = new byte[iPlayerCount];//各プレイヤーの位置
  static byte[] iAttack = new byte[iPlayerCount];//各プレイヤーの攻撃場所
  static string[] sAction = new string[iPlayerCount];

  public static void setLocatePast(int num, byte iInput){ iLocatePast[num] = iInput; }
  public static void setLocate(int num, byte iInput){ iLocate[num] = iInput; }
  public static void setAttack(int num, byte iInput){ iAttack[num] = iInput; }
  public static void setAction(int num, string sInput){ sAction[num] = sInput;}
  public static void setPlayerPos(int num, int iInput){ iPlayerPos[num] = iInput;}

  public static byte iShowLocatePast(int num){ return iLocatePast[num]; }
  public static byte iShowLocate(int num){ return iLocate[num]; }
  public static byte iShowAttack(int num){ return iAttack[num]; }
  public static int iShowPlayerPos(int num){ return iPlayerPos[num]; }

  public static byte iShowMyNumber(){ return iMyNumber; }

  static int[] iEndSelectAction = new int[iPlayerCount];

  public static int iShowEndSelectAction(int num){ return iEndSelectAction[num]; }

  int [] iEndAction = new int[iPlayerCount];

  int iSumSeq(int [] seq){
    int sum = 0;
    for(int i = 0; i < seq.Length; i++){
      sum += seq[i];
    }
    return sum;
  }

  void InitializeAction(){
    for(int i = 0; i < iPlayerCount; i++){
      sAction[i] = "none";
    }
  }

  void CalculatePosition(){
    int stageRadius = 30;//対戦場所の中心座標から岩までの半径
    int characterRadius = stageRadius + 10;

    float theta = 0;
    for(int i = 0; i < iRockNum; i++){
      //岩の座標
      v3RockPos[i] = new Vector3(v3StageCenter.x + stageRadius*Mathf.Cos(theta * Mathf.Deg2Rad),
                                 v3StageCenter.y,
                                 v3StageCenter.z + stageRadius*Mathf.Sin(theta * Mathf.Deg2Rad));
      //プレイヤーの待機場所の座標
      v3StandPos[i] = new Vector3(v3StageCenter.x + characterRadius*Mathf.Cos(theta * Mathf.Deg2Rad),
                                  v3StageCenter.y,
                                  v3StageCenter.z + characterRadius*Mathf.Sin(theta * Mathf.Deg2Rad));
      //射撃場所の座標
      v3TrgPos[i] = new Vector3(v3StageCenter.x + stageRadius*Mathf.Cos( (theta+(360.0f/iRockNum)/2) * Mathf.Deg2Rad),
                                v3StageCenter.y+5,
                                v3StageCenter.z + stageRadius*Mathf.Sin( (theta+(360.0f/iRockNum)/2) * Mathf.Deg2Rad));
      theta += 360.0f/iRockNum;
    }
  }

  Animator[] anim = new Animator[iPlayerCount];


  // Start is called before the first frame update
  void Start(){
    mainCamera = Camera.main.gameObject;
    PhotonNetwork.IsMessageQueueRunning = true;
    sMyName = SelectScene.GetCharacterName();//SelectSceneで選択したキャラクターをsMyNameに設定
    CalculatePosition();//岩やプレイヤーの位置の計算
    for(int i = 0; i < iRockNum; i++){
      PhotonNetwork.Instantiate("rock", v3RockPos[i], Quaternion.identity);//生成
    }

    //移動と攻撃の初期化
    for(int i = 0; i < iPlayerCount; i++){
      iLocatePast[i] = 0;
      iLocate[i] = 0;
      iAttack[i] = 0;
      iEndSelectAction[i] = 0;
    }

    var players = PhotonNetwork.PlayerList;//自分を含んだ全プレイヤーのデータの配列
    //選択したキャラクターを生成
    for(int i = 0; i < iPlayerCount; i++){
      GameObject goSelectedCharacter = goSelectCharacter(players[i].NickName);
      iPlayerPos[i] = MatchingScene.playerPosition[i];// 0 ~ iRockNum-1 までの乱数で指定したい
      iLocatePast[i] = iLocate[i] = (byte)iPlayerPos[i];
      goPlayer[i] = Instantiate(goSelectedCharacter, v3StandPos[iPlayerPos[i]], Quaternion.identity);
    }

    //カメラをキャラクターの外側のちょっと上に中心を見るように設置
    SetCameraTransPos(GameAction.v3CameraPos());
    SetCameraTransLook(v3StageCenter);

    text = text.GetComponent<Text>();
    text.text = $"{iSumSeq(iEndSelectAction)}/{iAlivePlayerCount}";

    for(int i = 0; i < iPlayerCount; i++){
      anim[i] = goPlayer[i].GetComponent <Animator> ();
      anim[i].SetFloat("Speed", 0f);
    }
    //Animation.SetWalkSpeed(0f);

  }

  // Update is called once per frame
  void Update(){
    for(int iPlayerNum = 0; iPlayerNum < iPlayerCount; iPlayerNum++){
      switch(sAction[iPlayerNum]){
      case "moveright":
        anim[iPlayerNum].SetFloat("Speed", fPlayerSpeed);
        GameAction.MoveRight(iPlayerNum);
        if(goPlayer[iPlayerNum].transform.position == v3StandPos[iLocate[iPlayerNum]]){
          anim[iPlayerNum].SetFloat("Speed", 0f);
          iEndAction[iPlayerNum] = 1;
        }
        break;

      case "moveleft":
        anim[iPlayerNum].SetFloat("Speed", fPlayerSpeed);
        GameAction.MoveLeft(iPlayerNum);
        if(goPlayer[iPlayerNum].transform.position == v3StandPos[iLocate[iPlayerNum]]){
          anim[iPlayerNum].SetFloat("Speed", 0f);
          iEndAction[iPlayerNum] = 1;
        }
        break;

      case "moverightdead":
        //moveright and dead
        anim[iPlayerNum].SetFloat("Speed", fPlayerSpeed);
        goPlayer[iPlayerNum].transform.position = Vector3.MoveTowards(goPlayer[iPlayerNum].transform.position, v3TrgPos[iLocate[iPlayerNum]], 100.0f * Time.deltaTime);
        if(goPlayer[iPlayerNum].transform.position == v3TrgPos[iLocate[iPlayerNum]]){
          Debug.Log("you dead");
          Vector3 pos = goPlayer[iPlayerNum].transform.position;
          pos.y += 5;
          goPlayer[iPlayerNum].transform.position = pos;
          anim[iPlayerNum].SetFloat("Speed", 0f);
          iAlivePlayerCount--;
        }
        break;

      case "moveleftdead":
        //moveleft and dead
        anim[iPlayerNum].SetFloat("Speed", fPlayerSpeed);
        goPlayer[iPlayerNum].transform.position = Vector3.MoveTowards(goPlayer[iPlayerNum].transform.position, v3TrgPos[(iLocate[iPlayerNum] - 1)%iRockNum], 100.0f * Time.deltaTime);
        if(goPlayer[iPlayerNum].transform.position == v3TrgPos[iLocate[iPlayerNum]]){
          Debug.Log("you dead");
          Vector3 pos = goPlayer[iPlayerNum].transform.position;
          pos.y += 5;
          goPlayer[iPlayerNum].transform.position = pos;
          anim[iPlayerNum].SetFloat("Speed", 0f);
          iAlivePlayerCount--;
        }
        break;

      case "waitdead":
			  //wait and dead
        break;

      case "shot":
        GameAction.Shot(iPlayerNum);
        if(/*Shot completed*/true){
          iEndAction[iPlayerNum] = 1;
        }
        break;
      }
    }
    switch(sAction[iMyNumber]){
      case "aim":
        //中心から、一つ隣の岩と今いる岩までのベクトル和の半分　つまり、二つの岩の真ん中になる。
        Vector3 v3ViewPos = (v3StandPos[iPlayerPos[iMyNumber]] + v3StandPos[(iPlayerPos[iMyNumber] +1)%iRockNum] - 2*v3StageCenter) * 0.4f;
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, v3StageCenter + v3ViewPos + v3CameraHeight, 100.0f * Time.deltaTime);
        mainCamera.transform.LookAt(v3TrgPos[(iPlayerPos[iMyNumber]+4)%iRockNum] + v3CameraHeight);//反対側の右寄りの岩にカメラを向けておく
        break;
      case "back":
        mainCamera.transform.position =
          Vector3.MoveTowards(mainCamera.transform.position,
                              v3StageCenter + 1.3f*(v3StandPos[iPlayerPos[iMyNumber]]-v3StageCenter) + v3CameraHeight,
                              100.0f * Time.deltaTime);
        mainCamera.transform.LookAt(v3StageCenter);
        break;
    }

    Debug.Log($"sum:{iSumSeq(iEndAction)}/pc{iPlayerCount}");
    if(iSumSeq(iEndAction) >= iAlivePlayerCount)OnStartTurn();
  }

  //ターン開始時の処理
  void OnStartTurn(){//initialize and decide action
    for(int i = 0; i < iPlayerCount; i++){
      //iLocate[i] = 0;
      iAttack[i] = 0;
      iEndAction[i] = 0;
      iEndSelectAction[i] = 0;
    }
    text.text = $"{iSumSeq(iEndSelectAction)}/{PhotonNetwork.CurrentRoom.PlayerCount}";
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
    //WhoKillWho();//calculate
    for(int iPlayerNum = 0; iPlayerNum < iPlayerCount; iPlayerNum++){
      if(iLocate[iPlayerNum] != iLocatePast[iPlayerNum]){//移動
        if((iLocate[iPlayerNum] < iLocatePast[iPlayerNum] && iLocate[iPlayerNum] != (byte)0) || (iLocate[iPlayerNum] > iLocatePast[iPlayerNum] && iLocate[iPlayerNum] == (byte)0)){
          sAction[iPlayerNum] = "moveleft";
        }else{
          sAction[iPlayerNum] = "moveright";
        }
      }else if(iAttack[iPlayerNum] == iRockNum + 1){
        sAction[iPlayerNum] = "wait";
      }else{
        sAction[iPlayerNum] = "shot";
      }
      Debug.Log($"OnEndTurn/{iPlayerNum}:{sAction[iPlayerNum]}, {iLocatePast[iPlayerNum]} -> {iLocate[iPlayerNum]}");
      //End:;
    }
    //run animation

    //this function call when user's action is decided.
    //so this should submit user's action.
  	WhoKillWho();
  	//↑で誰が誰を殺したかがわかるので、それによってActionを更新
  	//例)Actionがmoverightで殺されたら右に移動中に殺されるようにする
  	//.   Actionがshotで誰かを殺したら死ぬ奴を表示しながら殺すようにする
  	foreach(Result rPreResult in result){
  		if(rPreResult.iGetIsKill() == 1){
        sAction[rPreResult.iGetVictim()] += "dead";
  		}else if(rPreResult.iGetIsKill() == -1){
  			break;
  		}
  	}
  }

  enum Site{rock, target}

  class Result{
    public int victim, killer, site, position, isKill;
    public void InputResult(int v, int k, int s, int p, int isK){victim = v; killer = k; site = s; position = p; isKill = isK;}
    public void ShowResult(){
      //Debug.Log($"{killer} kill {victim} in {site}, {position}");
    }
  	public int iGetVictim(){return victim;}
  	public int iGetKiller(){return killer;}
  	public int iGetSite()  {return site;}
  	public int iGetPos()   {return position;}
  	public int iGetIsKill(){return isKill;}
  };

  Result[] result = new Result[16];

  void InitializeResult(){
    for(int i = 0; i < 16; i++){
      result[i] = new Result();
      result[i].isKill = -1;
    }
  }

  void WhoKillWho(){
    //Debug.Log("WhoKillWho-----");
    InitializeResult();
    int i = 0;
    for(int pn = 0; pn < iPlayerCount; pn++){
      for(int qn = 0; qn < iPlayerCount; qn++){
        if(pn!=qn){
          if(iLocate[qn] == iLocatePast[qn]){
            //player q is wait.
            if(iAttack[pn] == iLocate[qn]){
              //pn kill qn with knife.
              result[i].InputResult(qn, pn, (int)Site.rock, iAttack[pn], 1);
            }
          }else{
            //player q is move.
            int iPass;//通過した箇所
            if(iLocate[qn] > iLocatePast[qn] && iLocate[qn] != 0){
              iPass = iLocatePast[qn];
            }else{
              iPass = iLocatePast[qn]-1; if( iPass < 0 )iPass = iRockNum;
            }
            if(iAttack[pn] == iPass){
              //pn kill qn with gun.
              result[i].InputResult(qn, pn, (int)Site.target, iAttack[pn], 1);
            }
          }
          i++;
        }
      }
    }
    for(int j = 0; j < 16; j++) result[j].ShowResult();
  }

  //カスタムプロパティ関連
  public static void UpdateCustomPropaties(){
    //Debug.Log($"Update Custom Propaties / / iLocate:{iLocatePast[iMyNumber]}->{iLocate[iMyNumber]} / iAttack:{iAttack[iMyNumber]}");
    Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
    hashtable["iLocatePast"] = iLocatePast[iMyNumber];
    hashtable["iLocate"] = iLocate[iMyNumber];
    hashtable["iAttack"] = iAttack[iMyNumber];
    PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    hashtable.Clear();
  }//最初のプレイヤーのidは1のようです。

  public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){//カスタムプロパティが変更されるたびに呼び出されるらしい
    iLocatePast[targetPlayer.ActorNumber - 1] = (targetPlayer.CustomProperties["iLocatePast"] is byte value0) ? (byte)value0 : (byte)(iRockNum+1);
    iLocate[targetPlayer.ActorNumber - 1] = (targetPlayer.CustomProperties["iLocate"] is byte value1) ? (byte)value1 : (byte)(iRockNum+1);
    iAttack[targetPlayer.ActorNumber - 1] = (targetPlayer.CustomProperties["iAttack"] is byte value2) ? (byte)value2 : (byte)(iRockNum+1);
    Debug.Log($"ID:{targetPlayer.ActorNumber-1} / LP:{iLocatePast[targetPlayer.ActorNumber - 1]}, L:{iLocate[targetPlayer.ActorNumber - 1]}, A:{iAttack[targetPlayer.ActorNumber - 1]}");

    //全員のデータが集まるまで次のターンを開始しない
    if(iLocate[targetPlayer.ActorNumber - 1] != (iRockNum+1)){
      iEndSelectAction[targetPlayer.ActorNumber - 1] = 1;
      text.text = $"{iSumSeq(iEndSelectAction)}/{PhotonNetwork.CurrentRoom.PlayerCount}";
    }
    if(iSumSeq(iEndSelectAction)>=iAlivePlayerCount){
      OnEndTurn();
    }
  }

}
