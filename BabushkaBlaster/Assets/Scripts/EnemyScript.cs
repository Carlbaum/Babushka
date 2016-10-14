using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour {

  // PRIVATE VARIABLES
  private float startHP = 100;
  private float enemyHP = 100;
  private bool move = false;
  private bool isAlmostThere = false; //TODO: do this neater 
  private GenerateGrid gridScript;
  private Stack<Vector3> toGoTo;
  private InGameGUI gameCTRL;

  // Health bar specific variables
  private LineRenderer redLine;
  private LineRenderer greenLine;
//  private GameObject RedBar;
  private float barStart = -0.5f;
  private float barLength = 1.0f;
  private Vector3 barStartOffset;
  private Vector3 barEndOffset;
  private Vector3 barLengthOffset;

//  int health;

	Quaternion rotation;

  //PUBLIC VARIABLES
  public float turnSpeed = 20.0f;
  public float moveSpeed = 2.0f;
  public float destinationDistance = 0.5f;

  //PUBLIC FUNCTIONS

  public float getSpeed() { return moveSpeed; }

  public void OnTriggerEnter(Collider collider) {
    if(collider.gameObject.tag=="Projectile") {
      enemyHP -= collider.gameObject.GetComponent<ProjectileScript>().GetDamage();
      barLength = enemyHP/startHP;
//      Debug.Log ("Enemy hit!!\n" + enemyHP + " HP left!");
      if(enemyHP <= 0) {
        EnemyDeath();
      } 
    }
  }

  public void EnemyDeath()  {
    Destroy(gameObject); 
    //GameObject.FindGameObjectWithTag("GameController").GetComponent<GUI_InGame>().EnemyKilled();
    gameCTRL.EnemyKilled();
//    Debug.Log ("Enemies Killed: ");
  }


  // PRIVATE FUNCTIONS
	// Use this for initialization
	void Start () {
    gridScript = FindObjectOfType<GenerateGrid>();
    gameCTRL = GameObject.FindGameObjectWithTag("GameController").GetComponent<InGameGUI>();

    greenLine = gameObject.AddComponent<LineRenderer>();
    greenLine.material = new Material(Shader.Find("Toon/Basic Outline"));
    greenLine.material.SetColor("_Color",Color.green);
    greenLine.SetWidth(0.1f, 0.1f);
    greenLine.SetVertexCount(2);
    greenLine.SetColors(Color.red,Color.green);


    //RedBar = gameObject.GetComponentInChildren<LineRenderer>();
//    redLine = gameObject.AddComponent<LineRenderer>();
//    redLine.material = new Material(Shader.Find("Toon/Basic Outline"));
//    redLine.material.SetColor("_Color",Color.red);
//    redLine.SetWidth(0.1f, 0.1f);
//    redLine.SetVertexCount(2);
//    redLine.SetColors(Color.red,Color.green);

    barStartOffset = new Vector3(barStart, 2f, 0f);
    barEndOffset = new Vector3(1f, 2f, 0f);
//    barLengthOffset = new Vector3(barLength, 2f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
//    LineRenderer redLine = GetComponent<LineRenderer>();
//    redLine.SetPosition(0, gameObject.transform.position + barStartOffset);
//    redLine.SetPosition(1, gameObject.transform.position + barEndOffset);

//    LineRenderer lineRenderer = GetComponent<LineRenderer>();
    greenLine.SetPosition(0, gameObject.transform.position + barStartOffset);
    greenLine.SetPosition(1, gameObject.transform.position + new Vector3(barLength, 2f, 0f));

    if (move) {
      if (!isAlmostThere && Vector3.Distance(transform.position, toGoTo.Peek()) < destinationDistance) {	
        toGoTo.Pop();
//        print(transform.name + " Go towards" + toGoTo.Peek() + ", count: " + toGoTo.Count);
        if ( toGoTo.Count == 1) {
          isAlmostThere = true;
          turnSpeed *= 2;
//          moveSpeed /= 2;
        }
      }	
      rotation = Quaternion.LookRotation(toGoTo.Peek() - transform.position);
      transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
      //transform.LookAt(toGoTo.Peek());
      transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
      //transform.FindChild("Camera").LookAt(toGoTo.Peek());
      if (isAlmostThere && Vector3.Distance(transform.position, toGoTo.Peek()) < 0.1) {
        Destroy(gameObject); 
      }
    } else if (gridScript.GetIsPathFound()) {
      move = true;
      //TODO FIXthe toGoTo Stack.. right now the whole stack is being copied (and thus reversed) twice
//      toGoTo = gridScript.getShortestPath();
      toGoTo = new Stack<Vector3>(gridScript.getShortestPath());
      rotation = Quaternion.LookRotation(toGoTo.Peek() - transform.position);
			
    }		
	}
}
