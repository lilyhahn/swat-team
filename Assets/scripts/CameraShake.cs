using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	public float duration = 0.5f;
	public float speed = 1.0f;
	public float magnitude = 0.1f;
    bool shaking = false;
	
	
	public void PlayShake() {
        if(!shaking)
		    StartCoroutine("Shake");
	}
	
	private IEnumerator Shake() {
        shaking = true;
		float elapsed = 0.0f;
		
		Vector3 originalCamPos = transform.position;
		
		while (elapsed < duration && shaking) {
			elapsed += Time.deltaTime;
            transform.position += new Vector3(Random.Range(-magnitude, magnitude), Random.Range(-magnitude, magnitude), originalCamPos.z);
            yield return null;
		}
        transform.position = originalCamPos;
        shaking = false;
	}
}