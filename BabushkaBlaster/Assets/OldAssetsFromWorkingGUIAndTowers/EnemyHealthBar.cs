using UnityEngine;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour {
	GameObject enemy;
	LineRenderer HealthBar;
	// Use this for initialization
	void Start () {
		enemy = GameObject.FindGameObjectWithTag("Enemy");
	  
	}
	
	// Update is called once per frame
	void Update () {
		if(enemy) {
			Debug.Log("  " + enemy.name);
//			 HealthBar.SetPosition(0,enemy.transform.position + new Vector3(-0.5f,1f,0f));
//			 HealthBar.SetPosition(1,enemy.transform.position + new Vector3(0.5f,1f,0f));	
		}
	}
}
