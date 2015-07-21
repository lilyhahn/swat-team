using UnityEngine;
using System.Collections;

public class GnatSpawner : MonoBehaviour {
	public int maxGnats = 5;
	public GameObject gnat;
	public float checkInterval = 2f;
	
	void Start () {
		StartCoroutine(SpawnBugs());
	}
	
	void OnEnable(){
		StartCoroutine(SpawnBugs());
	}
	
	IEnumerator SpawnBugs(){
		while(true){
			if(GameObject.FindGameObjectsWithTag("gnat").Length < maxGnats){
				Instantiate(gnat, transform.position, Quaternion.identity);
			}
			yield return new WaitForSeconds(checkInterval);
		}
	}
}
