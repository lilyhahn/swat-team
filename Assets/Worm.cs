using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worm : Bug {
	
	Transform head;
	
	public List<GameObject> bodyParts;
	
	override protected void Start(){
		base.Start();
		head = transform.Find("Head");
	}
	
	public void Kill(GameObject part){
		Debug.Log(part);
		Debug.Log(bodyParts.IndexOf(part));
		for(int i = bodyParts.IndexOf(part)-1; i < bodyParts.Count; i++){
			bodyParts[i].GetComponent<HingeJoint2D>().enabled = false;
		}
	}

	override protected void Move(){
		if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0){
				if(Mathf.Abs(head.GetComponent<Rigidbody2D>().velocity.x) < maxSpeed)
					head.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Input.GetAxisRaw("Horizontal") * moveSpeed);
				else
					head.GetComponent<Rigidbody2D>().velocity = new Vector2(maxSpeed * Mathf.Sign(head.GetComponent<Rigidbody2D>().velocity.x), head.GetComponent<Rigidbody2D>().velocity.y);
			}
			else{
				head.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, head.GetComponent<Rigidbody2D>().velocity.y);
			}
			if(Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0){
				if(Mathf.Abs(head.GetComponent<Rigidbody2D>().velocity.y) < maxSpeed)
					head.GetComponent<Rigidbody2D>().AddForce(Vector2.up * Input.GetAxisRaw("Vertical") * moveSpeed);
				else
					head.GetComponent<Rigidbody2D>().velocity = new Vector2(head.GetComponent<Rigidbody2D>().velocity.x, maxSpeed * Mathf.Sign(head.GetComponent<Rigidbody2D>().velocity.y));
			}
			else{
				head.GetComponent<Rigidbody2D>().velocity = new Vector2(head.GetComponent<Rigidbody2D>().velocity.x, 0f);
			}
			/*if(Input.GetAxis("Vertical") != 0){
				head.GetComponent<Animator>().SetInteger("direction", (int)Mathf.Sign (Input.GetAxis("Vertical")));
				head.GetComponent<Animator>().SetTrigger("move");
			}
			if(Input.GetAxis("Horizontal") != 0){
				head.GetComponent<Animator>().SetTrigger("move");
			}*/
			float v = Input.GetAxisRaw("Vertical");
			float h = Input.GetAxisRaw("Horizontal");
			/*if(h == 0 && v == 0){
				head.eulerAngles = new Vector3(0, 0, head.eulerAngles.z - (head.eulerAngles.z % 45));
			}*/
			/*if(h > 0 && v == 0){
				if(GetComponent<Animator>().GetInteger("direction") > 0)
					head.eulerAngles = new Vector3(0, 0, -90 * Mathf.Sign (Input.GetAxis("Horizontal")));
				else if(GetComponent<Animator>().GetInteger("direction") < 0)
					head.eulerAngles = new Vector3(0, 0, 90 * Mathf.Sign (Input.GetAxis("Horizontal")));
			}*/
			/*else if(h < 0 && v == 0){
				if(GetComponent<Animator>().GetInteger("direction") > 0)
					head.eulerAngles = new Vector3(0, 0, -90 * Mathf.Sign (Input.GetAxis("Horizontal")));
				else if(GetComponent<Animator>().GetInteger("direction") < 0)
					head.eulerAngles = new Vector3(0, 0, 90 * Mathf.Sign (Input.GetAxis("Horizontal")));
			}*/
			if(h > 0 && v == 0){
				head.eulerAngles = new Vector3(0, 0, -90);
			}
			else if(h < 0 && v == 0){
				head.eulerAngles = new Vector3(0, 0, 90);
			}
			else if(h == 0 && v > 0){
				head.eulerAngles = new Vector3(0, 0, 0);
			}
			else if(h == 0 && v < 0){
				head.eulerAngles = new Vector3(0, 0, 180);
			}
			else if(h > 0 && v > 0){
				head.eulerAngles = new Vector3(0, 0, -45);
			}
			else if(h > 0 && v < 0){
				head.eulerAngles = new Vector3(0, 0, 225);
			}
			else if(h < 0 && v > 0){
				head.eulerAngles = new Vector3(0, 0, 45);
			}
			else if(h < 0 && v < 0){
				head.eulerAngles = new Vector3(0, 0, 135);
			}
	}
	
}
