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
