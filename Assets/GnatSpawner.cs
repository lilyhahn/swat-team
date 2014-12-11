using UnityEngine;
using System.Collections;

public class GnatSpawner : MonoBehaviour {
	public int maxGnats = 5;
	public GameObject gnat;
	public float checkInterval = 2f;
	
	void Start () {
		InvokeRepeating("SpawnBugs", 0, checkInterval);	
	}
	void SpawnBugs(){
		if(GameObject.FindGameObjectsWithTag("gnat").Length < maxGnats){
			Instantiate(gnat, transform.position, Quaternion.identity);
		}
	}
}
