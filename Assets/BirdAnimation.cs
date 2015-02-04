using UnityEngine;
using System.Collections;

public class BirdAnimation : MonoBehaviour {
		void SetTrigger(string trigger){
			transform.Find ("bird_graphics").GetComponent<Animator>().SetTrigger(trigger);
		}
}
