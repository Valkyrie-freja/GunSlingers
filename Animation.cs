using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour{
  //[SerializeField]
  private static Animator animator;
  // Start is called before the first frame update
  void Start(){
    animator = GetComponent <Animator> ();
  }

  public static void SetWalkSpeed(float speed){
    animator.SetFloat("Speed", speed);
  }

  // Update is called once per frame
  void Update(){

  }
}
