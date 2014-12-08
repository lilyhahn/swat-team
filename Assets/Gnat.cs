using UnityEngine;
using System.Collections;

public class Gnat : MonoBehaviour {
	Vector3 target;
	SpriteRenderer screen;
	void Start(){
		screen = GameObject.Find("level1").transform.Find ("screen").GetComponent<SpriteRenderer>();
		target = Random.insideUnitCircle * screen.bounds.extents.x;
	}
	void Update () {
		Vector3 dir = target - transform.position;
		float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		if(Vector3.Distance(transform.position, target) > .5f){
			rigidbody2D.velocity = transform.forward * 5f;
		}
	}
}
