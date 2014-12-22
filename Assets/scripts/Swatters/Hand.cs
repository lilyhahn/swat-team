using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour {
	public AudioClip squish;
	public AudioClip miss;
	public float stuckTime = 4f;
	protected Animator anim;
	protected bool stuck = false;
	Vector3 lastStuckPosition = Vector3.zero;
	protected void Start(){
		Screen.showCursor = false;
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if(stuck && GameObject.FindGameObjectWithTag("web").transform.position != lastStuckPosition){
			Debug.Log("lastStuckPosition: " + lastStuckPosition);
			Debug.Log("web position: " + GameObject.FindGameObjectWithTag("web").transform.position);
			stuck = false;
		}
		anim.SetBool("stuck", stuck);
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("swat")){
			anim.SetFloat("swatTime", anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
		}
		Vector3 mousePosition = Input.mousePosition;
		mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
		if(!anim.GetCurrentAnimatorStateInfo(0).IsName("swat") && !stuck)
			transform.position = Vector2.Lerp(transform.position, mousePosition, 1);
		if(Input.GetButtonDown("Fire1")){
			audio.clip = miss;
			audio.Play();
			Swat ();
		}
	}
	protected void OnTriggerEnter2D(Collider2D c){
		if(c.gameObject.tag == "web"){
			lastStuckPosition = c.transform.position;
			StartCoroutine(GetStuck());
		}
	}
	protected void OnTriggerStay2D(Collider2D c){
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("swat") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1){
			if(c.gameObject.tag == "bug"){
				GetComponent<AudioSource>().clip = squish;
				c.GetComponent<Bug>().Kill();
			}
			if(c.gameObject.tag == "gnat"){
				GetComponent<AudioSource>().clip = squish;
				c.GetComponent<Gnat>().Kill();
			}
			audio.Play();
		}
		if(Input.GetButtonDown("Fire1")){
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
		anim.SetTrigger("swat");
	}
}
