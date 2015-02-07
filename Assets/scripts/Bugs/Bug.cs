using UnityEngine;
using System.Collections;

public class Bug : MonoBehaviour {
	
	public float moveSpeed = 2f;
	public float maxSpeed = 5f;
	public AudioClip gnatEatSound;
	public AudioClip deathScream;
	public AudioClip specialSound;
	bool dead;
	bool inGame = true;
	float score = 0f;
	public float winningScore = 15;
	public float cooldown = 1f;
	public float finalCooldown = 0.5f;
	public float nextFire = 0.0f;
	
	float origMaxSpeed;
	float origMoveSpeed;
	
	void Start(){
		origMaxSpeed = maxSpeed;
		origMoveSpeed = moveSpeed;
	}
	
	// Update is called once per frame
	virtual protected void Update () {
		if(score >= winningScore && inGame){
			inGame = false;
			GameObject.Find("GameManager").GetComponent<GameManager>().EndGame(WinnerType.Bug);
		}
		if(!dead){
			if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0){
				if(Mathf.Abs(rigidbody2D.velocity.x) < maxSpeed)
					rigidbody2D.AddForce(Vector2.right * Input.GetAxisRaw("Horizontal") * moveSpeed);
				else
					rigidbody2D.velocity = new Vector2(maxSpeed * Mathf.Sign(rigidbody2D.velocity.x), rigidbody2D.velocity.y);
			}
			else{
				rigidbody2D.velocity = new Vector2(0f, rigidbody2D.velocity.y);
			}
			if(Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0){
				if(Mathf.Abs(rigidbody2D.velocity.y) < maxSpeed)
					rigidbody2D.AddForce(Vector2.up * Input.GetAxisRaw("Vertical") * moveSpeed);
				else
					rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, maxSpeed * Mathf.Sign(rigidbody2D.velocity.y));
			}
			else{
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
			}
			if(Input.GetAxis("Vertical") != 0){
				GetComponent<Animator>().SetInteger("direction", (int)Mathf.Sign (Input.GetAxis("Vertical")));
				GetComponent<Animator>().SetTrigger("move");
			}
			if(Input.GetAxis("Horizontal") != 0){
				//GetComponent<Animator>().SetInteger("direction", 1);
				GetComponent<Animator>().SetTrigger("move");
			}
			float v = Input.GetAxisRaw("Vertical");
			float h = Input.GetAxisRaw("Horizontal");
			if(h == 0 && v == 0){
				transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - (transform.eulerAngles.z % 45));
			}
			if(h == 1 && v == 0){
				if(GetComponent<Animator>().GetInteger("direction") == 1)
					transform.eulerAngles = new Vector3(0, 0, -90 * Mathf.Sign (Input.GetAxis("Horizontal")));
				else if(GetComponent<Animator>().GetInteger("direction") == -1)
					transform.eulerAngles = new Vector3(0, 0, 90 * Mathf.Sign (Input.GetAxis("Horizontal")));
			}
			else if(h == -1 && v == 0){
				if(GetComponent<Animator>().GetInteger("direction") == 1)
					transform.eulerAngles = new Vector3(0, 0, -90 * Mathf.Sign (Input.GetAxis("Horizontal")));
				else if(GetComponent<Animator>().GetInteger("direction") == -1)
					transform.eulerAngles = new Vector3(0, 0, 90 * Mathf.Sign (Input.GetAxis("Horizontal")));
			}
			if(h == 0 && v == 1){
				transform.rotation = Quaternion.identity;
			}
			else if(h == 0 && v == -1){
				transform.rotation = Quaternion.identity;
			}
			else if(h == 1 && v == 1){
				transform.eulerAngles = new Vector3(0, 0, -45);
			}
			else if(h == 1 && v == -1){
				transform.eulerAngles = new Vector3(0, 0, 45);
			}
			else if(h == -1 && v == 1){
				transform.eulerAngles = new Vector3(0, 0, 45);
			}
			else if(h == -1 && v == -1){
				transform.eulerAngles = new Vector3(0, 0, -45);
			}
			
			if(Input.GetButtonDown("Jump")){
				Special();
			}
		}
	}
	public void Kill(){
		dead = true;
		audio.clip = deathScream;
		audio.Play();
		GetComponent<Animator>().SetTrigger("dead");
		GameObject.Find("GameManager").GetComponent<GameManager>().EndGame(WinnerType.Human);
	}
	public void Reset(){
		dead = false;
		GetComponent<Animator> ().SetTrigger ("Reset");

	}
	virtual protected bool Special(){
		if (Time.time < nextFire) {
			return false;
		}
		nextFire = Time.time + cooldown;
		audio.clip = specialSound;
		audio.Play();
		return true;
	}
	protected virtual void OnTriggerEnter2D(Collider2D c){
		if(c.tag == "gnat" && !dead){
			audio.clip = gnatEatSound;
			audio.Play();
			score++;
			cooldown = Mathf.Lerp(cooldown, finalCooldown, score / winningScore);
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(2f, 2f), score / winningScore);
			Destroy(c.gameObject);
		}
	}
	protected virtual void OnCollisionEnter2D(Collision2D c){
		if(c.gameObject.tag == "web"){
			/*Debug.Log("collision");
			moveSpeed = moveSpeed / 2;
			maxSpeed = maxSpeed / 2;*/
			Physics2D.IgnoreCollision(c.gameObject.collider2D, collider2D);
		}
	}
	void OnCollisionExit2D(Collision2D c){
		if(c.gameObject.tag == "web"){
			moveSpeed = origMoveSpeed;
			maxSpeed = origMaxSpeed;
		}
	}
}
