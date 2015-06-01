using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour {
	public AudioClip squish;
	public AudioClip miss;
	public float stuckTime = 4f;
	public float gnatScatterRadius;
	public float stuckShakeDuration = 0.2f;
	public float stuckShakeMagnitude = 0.1f;
	bool shaking;
	protected Animator anim;
	protected bool stuck = false;
	Vector3 lastStuckPosition = Vector3.zero;
	protected virtual void Start(){
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("swat") && anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 0.8 && anim.GetCurrentAnimatorStateInfo (0).normalizedTime <= 1) {
			foreach(GameObject gnat in GameObject.FindGameObjectsWithTag("gnat")){
				if(Vector3.Distance(transform.position, gnat.transform.position) <= gnatScatterRadius){
					(gnat.GetComponent<Gnat>() as Gnat).Move(); // wtf
				}
			}
		}
		if(stuck && GameObject.FindGameObjectWithTag("web").transform.position != lastStuckPosition){
			Debug.Log("lastStuckPosition: " + lastStuckPosition);
			Debug.Log("web position: " + GameObject.FindGameObjectWithTag("web").transform.position);
			stuck = false;
		}
		anim.SetBool("stuck", stuck);
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("swat")){
			anim.SetFloat("swatTime", anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
		}
        if (Input.GetJoystickNames().Length < 2) {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("swat") && !stuck)
                transform.position = Vector2.Lerp(transform.position, mousePosition, 1);
        }
        else {
            transform.position += new Vector3(Input.GetAxis("Swatter X"), Input.GetAxis("Swatter Y"));
        }
		if(Input.GetButtonDown("Swat") && !anim.GetCurrentAnimatorStateInfo(0).IsName("swat")){
			GetComponent<AudioSource>().clip = miss;
			GetComponent<AudioSource>().Play();
			Swat ();
		}
	}
	protected void OnTriggerEnter2D(Collider2D c){
		if(c.gameObject.tag == "stinger"){
			StartCoroutine(GetStuck());
			Destroy(c.gameObject);
		}
	}
	protected void OnTriggerStay2D(Collider2D c){
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("swat") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1){
			Camera.main.GetComponent<CameraShake>().PlayShake();
			if(c.gameObject.tag == "bug"){
				GetComponent<AudioSource>().clip = squish;
				c.GetComponent<Bug>().Kill();
			}
			if(c.gameObject.tag == "worm"){
				GetComponent<AudioSource>().clip = squish;
				c.transform.GetComponent<WormPart>().Kill();
			}
			if(c.gameObject.tag == "gnat"){
				GetComponent<AudioSource>().clip = squish;
				c.GetComponent<Gnat>().Kill();
			}
			GetComponent<AudioSource>().Play();
		}
		if(Input.GetButtonDown("Swat")){
			if(c.gameObject.tag == "web"){
				lastStuckPosition = c.transform.position;
				StartCoroutine(GetStuck());
			}
		}
	}
	protected IEnumerator GetStuck(){
		stuck = true;
		yield return new WaitForSeconds(stuckTime);
		stuck = false;
	}
	protected virtual void Swat(){
		if(stuck){
			StartCoroutine(Shake());
			shaking = true;
		}
		anim.SetTrigger("swat");
	}
	
	IEnumerator Shake(){
		float elapsed = 0.0f;
		shaking = true;
		Vector3 origPos = transform.position;
		
		while (elapsed < stuckShakeDuration && shaking) {
			elapsed += Time.deltaTime;
            transform.position = origPos + new Vector3(Random.Range(-stuckShakeMagnitude, stuckShakeMagnitude), Random.Range(-stuckShakeMagnitude, stuckShakeMagnitude), origPos.z);
            yield return null;
		}
        transform.position = origPos;
		shaking = false;
	}
}
