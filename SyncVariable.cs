using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class SyncVariable : MonoBehaviourPunCallbacks{
  public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){
    //カスタムプロパティが更新されたプレイヤーの名前とIDをコンソールに出力する
    Debug.Log($"{targetPlayer.NickName}({targetPlayer.ActorNumber})");

    //更新されたプレイヤーのカスタムプロパティのペアをコンソールに出力する
    foreach(var prop in changedProps){
      Debug.Log($"{prop.Key}: {prop.Value}");
    }
  }

  public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged){
    //更新されたルームのカスタムプロパティのペアをコンソールに出力する
    foreach(var prop in propertiesThatChanged){
      Debug.Log($"{prop.Key}: {prop.Value}");
    }
  }
}
