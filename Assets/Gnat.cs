using UnityEngine;
using System.Collections;

public class Gnat : MonoBehaviour {
	Vector3 target;
	public float moveBounds;
	public float moveForce;
	public Sprite deadSprite;
	bool dead = false;
	void Start(){
		Move ();
	}
	void Update () {
		if(!dead){
			Vector3 dir = target - transform.position;
			float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
			rigidbody2D.AddForce((target - transform.position) * moveForce);
			if(Vector3.Distance(transform.position, target) < .5f){
				rigidbody2D.velocity = new Vector2(0, 0);
				Move();
			}
		}
	}
	void Move(){
		target = Random.insideUnitCircle * moveBounds;
	}
	public void Kill(){
		GetComponent<SpriteRenderer>().sprite = deadSprite;
		dead = true;
		rigidbody2D.velocity = new Vector2(0, 0);
		tag = "dead";
	}
}
