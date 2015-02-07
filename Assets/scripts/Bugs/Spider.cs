using UnityEngine;
using System.Collections;

public class Spider : Bug {
	public GameObject web;
	
	override protected bool Special(){
		if (!base.Special ())
			return true;
		Destroy(GameObject.FindGameObjectWithTag("web"));
		Instantiate(web, transform.position, Quaternion.identity);
		return true;
	}
}
