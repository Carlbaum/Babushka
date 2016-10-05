using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewEnemyScript : MonoBehaviour {

	int health; 
	public GameObject grid;
	bool move = false;
	Stack<Vector3> toGoTo;
	Quaternion rotation;
	public float turnSpeed = 2.0f,moveSpeed =2.0f,startTurn = 0.3f;

	// Use this for initialization
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update () {
		if	(move) {
//			print("Go towards" + toGoTo.Peek());
			if(Vector3.Distance(transform.position,toGoTo.Peek()) < startTurn) {	
				toGoTo.Pop();				
			}	
			rotation = Quaternion.LookRotation(toGoTo.Peek() - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
			transform.LookAt(toGoTo.Peek());
			transform.Translate(Vector3.forward * moveSpeed *Time.deltaTime);
			//transform.FindChild("Camera").LookAt(toGoTo.Peek());
		
		} else if(grid.GetComponent<NewGenerateGrid>().foundPath) {
			move = true;
			toGoTo = grid.GetComponent<NewGenerateGrid>().getShortestPath();
			rotation = Quaternion.LookRotation(toGoTo.Peek() - transform.position);
            print("FOUND PATH!!!!!!!");
		}	
		
		
	}
}
