using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAction : MonoBehaviour{
  static int iMyNumber = GameScene.iShowMyNumber();
  static float fPlayerSpeed = 10.0f * Time.deltaTime;
  static float fCameraSpeed = 15.0f * Time.deltaTime;

  //static Vector3 v3StageCenter = GameScene.v3ShowStageCenter();

  public static Vector3 v3CameraPos(){
    Vector3 v3CameraHeight = GameScene.v3ShowCameraHeight();
    Vector3 v3StageCenter = GameScene.v3ShowStageCenter();
    return v3StageCenter + 1.3f*(GameScene.v3ShowStandPos(GameScene.iShowPlayerPos(iMyNumber))-v3StageCenter) + v3CameraHeight;
  }

  public static void MoveRight(int playerID){
    //プレイヤーの位置の移動（カメラも一緒に移動させる）
    Vector3 v3StageCenter = GameScene.v3ShowStageCenter();
    GameScene.SetPlayerTransPos(playerID, Vector3.MoveTowards(GameScene.v3ShowPlayerTransPos(playerID), GameScene.v3ShowStandPos(GameScene.iShowLocate(playerID)), fPlayerSpeed));
    GameScene.SetPlayerTransLook(playerID, GameScene.v3ShowStandPos(GameScene.iShowLocate(playerID)));
    if(playerID == iMyNumber){
      GameScene.SetCameraTransPos(Vector3.MoveTowards(GameScene.v3ShowCameraTransPos(), v3CameraPos(), fCameraSpeed));
      GameScene.SetCameraTransLook(v3StageCenter);
    }
  }

  public static void MoveLeft(int playerID){
    //プレイヤーの位置の移動（カメラも一緒に移動させる）
    Vector3 v3StageCenter = GameScene.v3ShowStageCenter();
    GameScene.SetPlayerTransPos(playerID, Vector3.MoveTowards(GameScene.v3ShowPlayerTransPos(playerID), GameScene.v3ShowStandPos(GameScene.iShowLocate(playerID)), fPlayerSpeed));
    GameScene.SetPlayerTransLook(playerID, GameScene.v3ShowStandPos(GameScene.iShowLocate(playerID)));
    if(playerID == iMyNumber){
      GameScene.SetCameraTransPos(Vector3.MoveTowards(GameScene.v3ShowCameraTransPos(), v3CameraPos(), fCameraSpeed));
      GameScene.SetCameraTransLook(v3StageCenter);
    }
  }

  public static void Shot(int playerID){

  }

  public static void DeadAnimation(int playerID){}
  public static void KillAnimation(int playerID){}
}
