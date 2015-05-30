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
		for(int i = bodyParts.IndexOf(part)-1; i < bodyParts.Count; i++){
			bodyParts[i].GetComponent<HingeJoint2D>().enabled = false;
		}
	}

	override protected void Move(){
        Vector2 velocity = head.GetComponent<Rigidbody2D>().velocity;
		if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0){
            if (velocity.x < maxSpeed)
                velocity += Vector2.right * Input.GetAxisRaw("Horizontal");
            else
                velocity = new Vector2(Mathf.Sign(velocity.x), velocity.y);
			}
			else{
				velocity = new Vector2(0f, velocity.y);
			}
			if(Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0){
                if (Mathf.Abs(velocity.y) < maxSpeed)
                    velocity += Vector2.up * Input.GetAxisRaw("Vertical");
                else
                    velocity = new Vector2(velocity.x, Mathf.Sign(velocity.y));
			}
			else{
				velocity = new Vector2(velocity.x, 0f);
			}
            velocity = velocity.normalized * moveSpeed;
            head.GetComponent<Rigidbody2D>().velocity = velocity;
			float v = Input.GetAxisRaw("Vertical");
			float h = Input.GetAxisRaw("Horizontal");
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
