using UnityEngine;
using System.Collections;

public class Stinger : MonoBehaviour {

	public float speed;
	
	void Start(){
		GetComponent<Rigidbody2D>().velocity = -transform.up * speed;
	}
	void OnCollisionEnter2D(){
		Destroy(gameObject);
	}
}
