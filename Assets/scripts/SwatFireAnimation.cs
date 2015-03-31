using UnityEngine;
using System.Collections;

public class SwatFireAnimation : MonoBehaviour {
	void Update () {
		if(transform.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("swat")){
			GetComponent<Renderer>().enabled = true;
			GetComponent<Animator>().Play("fire");
		}
		else GetComponent<Renderer>().enabled = false;
	}
}
