using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour{

  int iMyNumber = (int)GameScene.iShowMyNumber();
  int iRockNum = GameScene.GetRockNum();

  public void OnAimButton(){
    GameScene.setAction(iMyNumber, "aim");
  }

  public void OnShotLeftButton(){
    GameScene.setLocatePast(iMyNumber, GameScene.iShowLocate(iMyNumber));
    GameScene.setLocate(iMyNumber, (byte)GameScene.iShowPlayerPos(iMyNumber));
    GameScene.setAttack(iMyNumber, (byte)((GameScene.iShowPlayerPos(iMyNumber) + 5) % iRockNum));
    GameScene.UpdateCustomPropaties();
  }

  public void OnShotCenterButton(){
    GameScene.setLocatePast(iMyNumber, GameScene.iShowLocate(iMyNumber));
    GameScene.setLocate(iMyNumber, (byte)GameScene.iShowPlayerPos(iMyNumber));
    GameScene.setAttack(iMyNumber, (byte)((GameScene.iShowPlayerPos(iMyNumber) + 4) % iRockNum));
    GameScene.UpdateCustomPropaties();
  }

  public void OnShotRightButton(){
    GameScene.setLocatePast(iMyNumber, GameScene.iShowLocate(iMyNumber));
    GameScene.setLocate(iMyNumber, (byte)GameScene.iShowPlayerPos(iMyNumber));
    GameScene.setAttack(iMyNumber, (byte)((GameScene.iShowPlayerPos(iMyNumber) + 3) % iRockNum));
    GameScene.UpdateCustomPropaties();
  }

  public void OnRightButton(){
    if(GameScene.iShowEndSelectAction(iMyNumber) == 0){
      int iCurrentPos = GameScene.iShowPlayerPos(iMyNumber);
      GameScene.setPlayerPos(iMyNumber, (iCurrentPos+1) % iRockNum);
      GameScene.setLocatePast(iMyNumber, (byte)iCurrentPos);
      GameScene.setLocate(iMyNumber, (byte)GameScene.iShowPlayerPos(iMyNumber));
      GameScene.setAttack(iMyNumber, (byte)GameScene.iShowPlayerPos(iMyNumber));
      GameScene.UpdateCustomPropaties();
    }
  }

  public void OnLeftButton(){
    if(GameScene.iShowEndSelectAction(iMyNumber) == 0){
      int iCurrentPos = GameScene.iShowPlayerPos(iMyNumber);
      GameScene.setPlayerPos(iMyNumber, (iCurrentPos-1 +iRockNum) % iRockNum);
      GameScene.setLocatePast(iMyNumber, (byte)iCurrentPos);
      GameScene.setLocate(iMyNumber, (byte)GameScene.iShowPlayerPos(iMyNumber));
      GameScene.setAttack(iMyNumber, (byte)GameScene.iShowPlayerPos(iMyNumber));
      GameScene.UpdateCustomPropaties();
    }
  }

  public void OnBackButton(){
    GameScene.setAction(iMyNumber, "back");
  }

  public void OnWaitButton(){
    GameScene.setLocatePast(iMyNumber, GameScene.iShowLocate(iMyNumber));
    GameScene.setLocate(iMyNumber, (byte)GameScene.iShowPlayerPos(iMyNumber));
    GameScene.setAttack(iMyNumber, (byte)(iRockNum + 1));
    GameScene.UpdateCustomPropaties();
  }
}
