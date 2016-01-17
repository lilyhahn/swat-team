using UnityEngine;
using System.Collections;

public class SwatFireAnimation : MonoBehaviour {
	Animator anim;

	void Start(){
		anim = transform.parent.GetComponent<Animator>();
	}
	void Update () {
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("swat")){
			GetComponent<Renderer>().enabled = true;
			GetComponent<Animator>().Play("fire");
		}
		else GetComponent<Renderer>().enabled = false;
	}
}
