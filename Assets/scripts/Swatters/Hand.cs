using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour {
	public AudioClip squish;
	public AudioClip miss;
	public float stuckTime = 4f;
	public float gnatScatterRadius;
	public float stuckShakeDuration = 0.2f;
	public float stuckShakeMagnitude = 0.1f;
    public float swatTime = 0.7f;
    public float swatTimeEnd = 0.8f;
	public float sensitivity = 1f;
    public Animator hit;
    public Color webStuckColor;
	bool shaking;
	protected Animator anim;
	protected bool stuck = false;
	Vector3 lastStuckPosition = Vector3.zero;
	Vector3 lastAnimPosition = Vector3.zero;
	Vector3 firstAnimPosition;
	protected virtual void Start(){
		anim = transform.Find("anim").GetComponent<Animator>();
		firstAnimPosition = anim.transform.localPosition;
        transform.Find("control").GetComponent<ControlDisplay>().Fade();
	}
	
	// Update is called once per frame
	protected virtual void FixedUpdate () {
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("swat") && anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= swatTime && anim.GetCurrentAnimatorStateInfo (0).normalizedTime <= swatTimeEnd) {
			Camera.main.GetComponent<CameraShake>().PlayShake(0.5f, 0.5f, 0.05f);
			Instantiate(hit, anim.transform.position, Quaternion.identity);
            foreach(GameObject gnat in GameObject.FindGameObjectsWithTag("gnat")){
				if(Vector3.Distance(transform.position, gnat.transform.position) <= gnatScatterRadius){
					(gnat.GetComponent<Gnat>() as Gnat).Move(); // wtf
				}
			}
		}
		if(GameObject.FindGameObjectWithTag("web") != null && stuck && GameObject.FindGameObjectWithTag("web").transform.position != lastStuckPosition){
			Debug.Log("lastStuckPosition: " + lastStuckPosition);
			Debug.Log("web position: " + GameObject.FindGameObjectWithTag("web").transform.position);
            anim.SetTrigger("unstuck");
			stuck = false;
            anim.GetComponent<SpriteRenderer>().color = Color.white;
		}
		anim.SetBool("stuck", stuck);
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("swat")){
			anim.SetFloat("swatTime", anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
		}
	}
    protected virtual void Update(){
        if (GameObject.Find("GameManager").GetComponent<GameManager>().paused) {
            anim.speed = 0;
        }
        else {
            anim.speed = 1;
        }
        lastAnimPosition = anim.transform.position;
        if(!GameObject.Find("GameManager").GetComponent<GameManager>().paused){
            //transform.position += new Vector3(Input.GetAxis("Mouse X") * Time.deltaTime, Input.GetAxis("Mouse Y") * Time.deltaTime) * sensitivity;
            //transform.position += new Vector3(Input.GetAxis("Swatter X"), Input.GetAxis("Swatter Y"));
            // temp for wii remote
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
		    if(Input.GetButtonDown("Swat") && !anim.GetCurrentAnimatorStateInfo(0).IsName("swat")){
			    GetComponent<AudioSource>().clip = miss;
			    GetComponent<AudioSource>().Play();
			    Swat ();
		    }
        }
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("swat") || stuck) {
			anim.transform.position = lastAnimPosition;
		}
		else {
			anim.transform.localPosition = firstAnimPosition;
		}
    }
	public void OnTriggerEnter2D(Collider2D c){
		if(c.gameObject.tag == "stinger"){
			StartCoroutine(GetStuck());
			Destroy(c.gameObject);
		}
	}
	public void OnTriggerStay2D(Collider2D c){
		if(!stuck && anim.GetCurrentAnimatorStateInfo(0).IsName("swat") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= swatTime && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= swatTimeEnd){
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
            if(c.gameObject.tag == "web"){
				lastStuckPosition = c.transform.position;
                anim.GetComponent<SpriteRenderer>().color = webStuckColor;
				StartCoroutine(GetStuck());
			}
			GetComponent<AudioSource>().Play();
		}
        //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), c); // only hit once
	}
    public void OnTriggerExit2D(Collider2D c) {
        //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), c, false);
    }
	protected IEnumerator GetStuck(){
		stuck = true;
		yield return new WaitForSeconds(stuckTime);
		stuck = false;
        anim.SetTrigger("unstuck");
        anim.GetComponent<SpriteRenderer>().color = Color.white;
	}
	protected virtual void Swat(){
		if(stuck){
			StartCoroutine(Shake());
			shaking = true;
            return;
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
