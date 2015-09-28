using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour {
	
	public float fadeSpeed = 0.25f;
	
	public void Fade(){
		LeanTween.alpha(gameObject, 0f, fadeSpeed).setOnComplete(delegate(){ Destroy(gameObject); } );
	}
	
}
