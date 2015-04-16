﻿using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	public float duration = 0.5f;
	public float speed = 1.0f;
	public float magnitude = 0.1f;
    bool shaking = false;
    float oldDuration;
    float oldSpeed;
    float oldMagnitude;
	
	public void PlayShake() {
        oldDuration = duration;
        oldSpeed = speed;
        oldMagnitude = magnitude;
        if(!shaking)
		    StartCoroutine("Shake");
	}
    public void PlayShake(float Duration, float Speed, float Magnitude) {
        if (Duration > 0) {
            oldDuration = duration;
            duration = Duration;
        }
        if (Speed > 0) {
            oldSpeed = speed;
            speed = Speed;
        }
        if (Magnitude > 0) {
            oldMagnitude = magnitude;
            magnitude = Magnitude;
        }
        if (!shaking)
            StartCoroutine(Shake());
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
        speed = oldSpeed;
        duration = oldDuration;
        magnitude = oldMagnitude;
        shaking = false;
	}
}