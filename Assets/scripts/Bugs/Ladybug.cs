using UnityEngine;
using System.Collections;

public class Ladybug : Bug {
	public float rollSpeedMultiplier = 3f;
	protected override bool Special(){
		if (!base.Special ())
			return true;
		StartCoroutine(Roll ());
		return true;
	}
	IEnumerator Roll(){
		float maxSpeedOld = maxSpeed;
		maxSpeed = Mathf.Infinity;
		GetComponent<Rigidbody2D>().AddForce(transform.up * moveSpeed * rollSpeedMultiplier * GetComponent<Animator>().GetInteger("direction"));
		yield return new WaitForSeconds(0.25f);
		maxSpeed = maxSpeedOld;
		/*Vector3 target = transform.up * rollDistance + transform.position;
		Debug.Log (target);
		while(Vector3.Distance(transform.position, target) > 0.3){
			transform.position = Vector3.Lerp (transform.position, target, Time.deltaTime * rollSpeedMultiplier);
			yield return null;
		}*/
		/*if(Input.GetAxis("Horizontal") < 0){
			float origX = transform.position.x;
			while(transform.position.x - origX > origX - rollDistance){
				transform.position = new Vector3(transform.position.x - moveSpeed * rollSpeedMultiplier, transform.position.y);
				yield return null;
			}
		}
		else if(Input.GetAxis("Horizontal") > 0){
			float origX = transform.position.x;
			while(transform.position.x + origX < origX + rollDistance){
				transform.position = new Vector3(transform.position.x + moveSpeed * rollSpeedMultiplier, transform.position.y);
				yield return null;
			}
		}
		else if(Input.GetAxis("Vertical") < 0){
			float origY = transform.position.y;
			while(transform.position.y - origY > origY - rollDistance){
				transform.position = new Vector3(transform.position.x, transform.position.y - moveSpeed * rollSpeedMultiplier);
				yield return null;
			}
		}
		else if(Input.GetAxis("Vertical") > 0){
			float origY = transform.position.y;
			while(transform.position.y + origY < origY + rollDistance){
				transform.position = new Vector3(transform.position.x, transform.position.y + moveSpeed * rollSpeedMultiplier);
				yield return null;
			}
		}*/
	}
}
