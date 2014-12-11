using UnityEngine;
using System.Collections;

public class Spider : Bug {
	public GameObject web;
	
	override protected void Special(){
		base.Special();
		Destroy(GameObject.FindGameObjectWithTag("web"));
		Instantiate(web, transform.position, Quaternion.identity);
	}
}
