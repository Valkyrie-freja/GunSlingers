using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
  Vector3 firstPosition = GameScene.v3StageCenter;

  // Start is called before the first frame update
  void Start(){
    //Instantiate(this.gameObject, firstPosition, Quaternion.identity);
  }

  // Update is called once per frame
  void Update(){
    var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    this.gameObject.transform.Translate(6f * Time.deltaTime * input.normalized);
  }
}
