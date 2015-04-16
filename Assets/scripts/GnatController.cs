using UnityEngine;
using System.Collections;

public class GnatController : MonoBehaviour {
	public Vector3 target;
	public Vector2 moveBounds;
	public float minMoveDistance = 1;
	
	void Start(){
		Move();
	}
	
	public void Move(){
		StartCoroutine (MoveRoutine ());
	}
	
	IEnumerator MoveRoutine(){
		target = new Vector3 (Random.Range (-moveBounds.x, moveBounds.x), Random.Range (-moveBounds.y, moveBounds.y));
		while ((Vector3.Distance (target, transform.position) < minMoveDistance)) {
			target = new Vector3 (Random.Range (-moveBounds.x, moveBounds.x), Random.Range (-moveBounds.y, moveBounds.y));
			yield return null;
		}
	}
}
