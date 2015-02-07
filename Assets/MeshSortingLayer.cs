using UnityEngine;
using System.Collections;

public class MeshSortingLayer : MonoBehaviour {
	
	public string sortingLayer;
	public int sortingOrder = 0;
	
	// Use this for initialization
	void Start () {
		GetComponent<MeshRenderer>().sortingLayerName = sortingLayer;
		GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
	}
}
