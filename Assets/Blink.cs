using UnityEngine;
using System.Collections;

public class Blink : MonoBehaviour {
	
	public float blinkInterval;

	void Start () {
		InvokeRepeating ("ToggleRenderer", 0, blinkInterval);
	}
	
	void ToggleRenderer(){
		GetComponent<Renderer> ().enabled = !GetComponent<Renderer> ().enabled;
	}
}
