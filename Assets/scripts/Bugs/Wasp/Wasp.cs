using UnityEngine;
using System.Collections;

public class Wasp : Bug {
	public Animator wings;
	
	public Stinger stinger;
	
	protected override void Move(){
		base.Move();
		float v = Input.GetAxisRaw("Vertical");
		float h = Input.GetAxisRaw("Horizontal");
		if(h > 0 && v == 0){
			transform.eulerAngles = new Vector3(0, 0, 270);
		}
		else if(h < 0 && v == 0){
			transform.eulerAngles = new Vector3(0, 0, 90);
		}
		if(h == 0 && v < 0){
			transform.eulerAngles = new Vector3(0, 0, 180);
		}
		if(h < 0 && v < 0){
			transform.eulerAngles = new Vector3(0, 0, 135);
		}
		if(h > 0 && v < 0){
			transform.eulerAngles = new Vector3(0, 0, 225);
		}
		if(Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0){
			wings.SetTrigger("moving");
		}
		else{
			wings.SetTrigger("idle");
		}
	}
	protected override bool Special(){
		if(!base.Special()){
			return false;
		}
		Instantiate(stinger, transform.position, transform.rotation);
		return true;
	}
}
