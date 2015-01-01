using UnityEngine;
using System.Collections;

public class SwatFireAnimation : MonoBehaviour {
	void Update () {
		if(transform.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("swat")){
			renderer.enabled = true;
			GetComponent<Animator>().Play("fire");
		}
		else renderer.enabled = false;
	}
}
