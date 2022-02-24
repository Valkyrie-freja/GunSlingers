using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){

    }

    // Update is called once per frame
    void Update(){

    }

    protected static string cname = "AvatarFox";
    protected static int iCharacterId = 0;

    /*public void SelectCharacter(int num){
      iCharacterId = num;
    }*/

    public static int GetCharacterId(){
      return iCharacterId;
    }

    public void SelectCharacter(string name){
      cname=name;
    }

    public void ChangeScene(){
      SceneManager.LoadScene("MatchingScene");
    }

    public static string GetCharacterName(){
      return cname;
    }
}
