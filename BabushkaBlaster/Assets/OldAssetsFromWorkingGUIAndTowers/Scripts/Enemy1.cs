using UnityEngine;
using System.Collections;

public class Enemy1 : MonoBehaviour {
	
	public static float 	mySpeed = 5f, 
							turnSpeed = 80.0f;
	private float 			startHP = 100,
				 			enemyHP = 100;
//	private LineRenderer 	healthBar;
//	public GameObject 		EnemyHealthObj;
	private GameObject		RedBar;
	private float 			barStart = -0.5f,
							barLength = 1.0f;
	private GUI_InGame		gameCTRL;

	// Use this for initialization
	void Start () 
	{
		
		//LineRenderer lineRenderer = healthBar.GetComponent<HealthBar>();
		
		
	/*	LineRenderer lineRendDmg = gameObject.AddComponent<LineRenderer>();
	    lineRendDmg.material = new Material(Shader.Find("Toon/Basic Outline"));
	   	lineRendDmg.material.SetColor("_Color",Color.red);
	    lineRendDmg.SetWidth(0.1f, 0.1f);
	    lineRendDmg.SetVertexCount(2);
	*/	
		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
	    lineRenderer.material = new Material(Shader.Find("Toon/Basic Outline"));
	   	lineRenderer.material.SetColor("_Color",Color.green);
	    lineRenderer.SetWidth(0.1f, 0.1f);
	    lineRenderer.SetVertexCount(2);
		lineRenderer.SetColors(Color.red,Color.green);
		
		gameCTRL = GameObject.FindGameObjectWithTag("GameController").GetComponent<GUI_InGame>();
		
		//RedBar = gameObject.GetComponentInChildren<LineRenderer>();
		LineRenderer rB = gameObject.GetComponentInChildren<LineRenderer>();
	    rB.material = new Material(Shader.Find("Toon/Basic Outline"));
	   	rB.material.SetColor("_Color",Color.red);
	    rB.SetWidth(0.1f, 0.1f);
	    rB.SetVertexCount(2);
		rB.SetColors(Color.red,Color.green);
		/*Instantiate(EnemyHealthObj);
		Debug.Log (EnemyHealthObj.GetComponent("LineRenderer").GetType());
		healthBar = EnemyHealthObj.GetComponent<LineRenderer>();
		//testLineRend.SetColors(Color.(0, 1, 0, 1),(0, 1, 0, 1));
		if(healthBar) 
		{
			Debug.Log("Found health bar" + healthBar.GetType());
			healthBar.SetWidth(0.1f,0.4f);
			healthBar.SetVertexCount(2);
			//healthBar.SetPosition(0, transform.position +new Vector3(-0.5f,1f,0f));
			//healthBar.SetPosition(1, transform.position +new Vector3(0.5f,1f,0f));
	//		
		}*/
		
	}
	
	// Update is called once per frame
	void Update () {
		LineRenderer rB = GetComponent<LineRenderer>();
        rB.SetPosition(0, gameObject.transform.position +new Vector3(barStart,1f,0f));
		rB.SetPosition(1, gameObject.transform.position +new Vector3(1f,1f,0f));
		
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, gameObject.transform.position +new Vector3(barStart,1f,0f));
		lineRenderer.SetPosition(1, gameObject.transform.position +new Vector3(barLength,1f,0f));
		
//		lineRenderer.SetPositions = transform.position + new Vector3(0f,1f,0f);
		//Debug.Log(" " + gameObject.transform.position);
		
		if(Input.GetKey(KeyCode.UpArrow)) 
			transform.Translate(Vector3.forward * mySpeed *Time.deltaTime);
		if(Input.GetKey(KeyCode.DownArrow)) 
			transform.Translate(-Vector3.forward * mySpeed *Time.deltaTime);
		if(Input.GetKey(KeyCode.LeftArrow)) 
			transform.Rotate(Vector3.up * -turnSpeed *Time.deltaTime);
		if(Input.GetKey(KeyCode.RightArrow)) 
			transform.Rotate(Vector3.up * turnSpeed *Time.deltaTime);
		if(Input.GetKey(KeyCode.W)) 
			transform.Rotate(Vector3.left * turnSpeed *Time.deltaTime);
		if(Input.GetKey(KeyCode.S)) 
			transform.Rotate(Vector3.left * -turnSpeed *Time.deltaTime);
		if(Input.GetKey(KeyCode.P)) 
		{
			transform.position = new Vector3(0,0.5f,6.0f);
			//transform.rotation = Quaternion.identity;
			transform.rotation.Set(0,180,0,0);
		}
		/*testLineRend.SetPosition(0, transform.position +new Vector3(-0.5f,1f,0f));
		testLineRend.SetPosition(1, transform.position +new Vector3(0.5f,1f,0f));
		testLineRend.material.SetColor("_Color",Color.green);*/
      //  testLineRend.material.SetColor.
		//testLineRend.SetWidth(0.1f,0.1f);
		
		
	}
	
	public void OnTriggerEnter(Collider collider) 
	{
		if(collider.gameObject.tag=="Projectile") {
			enemyHP -= collider.gameObject.GetComponent<ProjectileScript>().GetDamage();
			barLength = enemyHP/startHP;
			Debug.Log ("Enemy hit!!\n" + enemyHP + " HP left!");
			if(enemyHP <= 0)
			{
				EnemyDeath();
			}	
		}
	}
	
	public void EnemyDeath() 
	{
		
		Destroy(gameObject); 
		//GameObject.FindGameObjectWithTag("GameController").GetComponent<GUI_InGame>().EnemyKilled();
		gameCTRL.EnemyKilled();
		Debug.Log ("Enemies Killed: ");
		
	}
	
	
	
}
