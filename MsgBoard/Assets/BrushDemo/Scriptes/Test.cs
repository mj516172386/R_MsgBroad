using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
	public GameObject initafda;
	public Transform canvas;
	// Use this for initialization
	void Start () {
		 initafda = Instantiate(Resources.Load<GameObject>("Prefabs/Panel"),canvas);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) 
		{
		    DestroyImmediate(initafda);
			initafda = Instantiate(Resources.Load<GameObject>("Prefabs/Panel"), canvas);
		}
	}
}
