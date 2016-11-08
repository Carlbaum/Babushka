using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour {

  // PRIVATE VARIABLES
  private float startHP = 100;
  private float enemyHP = 100;
  private bool move = false;
  private GridHandler gridScript;
  private Stack<Vector3> checkpointPosition;
  private InGameGUI gameCTRL;

  // Health bar specific variables
  private LineRenderer redLine;
  private LineRenderer greenLine;
//  private GameObject RedBar;
  private float barStart = -0.5f;
  private float barLength = 1.0f;
  private Vector3 barStartOffset;
//  private Vector3 barEndOffset;
  private Vector3 barLengthOffset;

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
    gameCTRL.EnemyKilled();
  }


  // PRIVATE FUNCTIONS
	// Use this for initialization
	void Start () {
    gridScript = FindObjectOfType<GridHandler>();
    gameCTRL = GameObject.FindGameObjectWithTag("GameController").GetComponent<InGameGUI>();

    greenLine = gameObject.AddComponent<LineRenderer>();
    greenLine.material = new Material(Shader.Find("Toon/Basic Outline"));
    greenLine.material.SetColor("_Color",Color.green);
    greenLine.SetWidth(0.1f, 0.1f);
    greenLine.SetVertexCount(2);
    greenLine.SetColors(Color.red,Color.green);

    barStartOffset = new Vector3(barStart, 2f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
    greenLine.SetPosition(0, gameObject.transform.position + barStartOffset);
    greenLine.SetPosition(1, gameObject.transform.position + new Vector3(barLength, 2f, 0f));

    if (move) {
      if (checkpointPosition.Count > 1 && Vector3.Distance(transform.position, checkpointPosition.Peek()) < destinationDistance) {	
        checkpointPosition.Pop();
      }	
      rotation = Quaternion.LookRotation(checkpointPosition.Peek() - transform.position);
      transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
      transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
      if (checkpointPosition.Count == 1 && Vector3.Distance(transform.position, checkpointPosition.Peek()) < 0.1) {
        Destroy(gameObject); 
        gameCTRL.addPlayerHealth(-1);
      }
    } else if (gridScript.GetIsPathFound()) {
      move = true;
      //TODO FIX the checkpointPosition Stack.. right now the whole stack is being copied (and thus reversed) twice
//      checkpointPosition = gridScript.getShortestPath();
      checkpointPosition = new Stack<Vector3>(gridScript.getShortestPath());
      rotation = Quaternion.LookRotation(checkpointPosition.Peek() - transform.position);
			
    }		
	}
}
