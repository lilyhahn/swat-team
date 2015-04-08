using UnityEngine;
using System.Collections;

public class Gnat : MonoBehaviour {
	Vector3 target;
	public Vector2 moveBounds;
	public float moveForce;
	public Sprite deadSprite;
	public float minMoveDistance = 1;
	bool dead = false;
	public bool stuck = false;

	void Start(){
		Move ();
	}
	void Update () {
		if(!dead && !stuck && target != Vector3.zero){
			Vector3 dir = target - transform.position;
			float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
			GetComponent<Rigidbody2D>().AddForce((target - transform.position) * moveForce);
			if(Vector3.Distance(transform.position, target) < .5f){
				GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
				Move();
			}
		}
	}
	public void Move(){
		StartCoroutine (MoveRoutine ());
	}
	IEnumerator MoveRoutine(){
		target = new Vector3 (Random.Range (-moveBounds.x, moveBounds.x), Random.Range (-moveBounds.y, moveBounds.y));
		while ((Vector3.Distance (target, transform.position) < minMoveDistance)) {
			target = new Vector3 (Random.Range (-moveBounds.x, moveBounds.x), Random.Range (-moveBounds.y, moveBounds.y));
			yield return null;
		}
	}
	public void Kill(){
		GetComponent<SpriteRenderer>().sprite = deadSprite;
		dead = true;
		GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
		tag = "dead";
	}
	void OnTriggerStay2D(Collider2D c){
		if (c.tag == "web") {
			Debug.Log("gnat stuck");
			stuck = true;
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
		}
	}
}
