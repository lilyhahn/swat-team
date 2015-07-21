using UnityEngine;
using System.Collections;

public class Gnat : MonoBehaviour {
	Vector3 target;
	public float moveForce;
	public Sprite deadSprite;
	bool dead = false;
	public bool stuck = false;
    Vector3 lastStuckPosition = Vector3.zero;
	public float bugRunDistance = 1f;
	GnatController controller;
	Transform bug;

	void Start(){
		controller = GameObject.Find ("GnatController").GetComponent<GnatController>();
		bug = GameObject.FindGameObjectWithTag("bug").transform;
		//Move ();
	}
	void Update () {
		target = controller.target;
        if (stuck && GameObject.FindGameObjectWithTag("web").transform.position != lastStuckPosition) {
            stuck = false;
            GetComponent<Rigidbody2D>().isKinematic = false;
        }
		if(Vector3.Distance(transform.position, bug.position) < bugRunDistance){
			Move();
		}
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
		//StartCoroutine (MoveRoutine ());
		controller.Move();
	}
	public void Kill(){
		GetComponent<SpriteRenderer>().sprite = deadSprite;
		dead = true;
		GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
		GetComponent<Rigidbody2D>().isKinematic = true;
		GetComponent<Collider2D>().enabled = false;
		tag = "dead";
	}
	void OnCollisionEnter2D(Collision2D c){
		if (c.gameObject.tag == "web") {
			stuck = true;
            lastStuckPosition = c.transform.position;
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			GetComponent<Rigidbody2D>().isKinematic = true;
		}
	}
}
