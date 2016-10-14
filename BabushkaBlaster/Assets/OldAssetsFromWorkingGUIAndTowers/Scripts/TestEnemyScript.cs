using UnityEngine;
using System.Collections;

public class TestEnemyScript : MonoBehaviour {
	
	public float moveSpeed = 2.0f, turnSpeed = 40.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.UpArrow)) 
			transform.Translate(Vector3.forward * moveSpeed *Time.deltaTime);
		if(Input.GetKey(KeyCode.DownArrow)) 
			transform.Translate(-Vector3.forward * moveSpeed *Time.deltaTime);
		if(Input.GetKey(KeyCode.LeftArrow)) 
			transform.Rotate(Vector3.up * -turnSpeed *Time.deltaTime);
		if(Input.GetKey(KeyCode.RightArrow)) 
			transform.Rotate(Vector3.up * turnSpeed *Time.deltaTime);
		if(Input.GetKey(KeyCode.W)) 
			transform.Rotate(Vector3.left * turnSpeed *Time.deltaTime);
		if(Input.GetKey(KeyCode.S)) 
			transform.Rotate(Vector3.left * -turnSpeed *Time.deltaTime);
	}
}
