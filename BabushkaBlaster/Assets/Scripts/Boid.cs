using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : Enemy {
  // PRIVATE VARIABLES
  private Stack<Vector3> checkpoints;

  //PUBLIC VARIABLES
  public Vector3 speedDiff;
  public Vector3 averagePosition;
  public Vector3 avoidanceVector;

  public Vector3 boidVector;

  public float boidWeight        = 1.0f; 
  public float cohesionDistance  = 1.2f;
  public float avoidanceDistance = 0.6f;
  public float alignmentWeight   = 0.1f;
  public float cohesionWeight    = 0.45f;
  public float avoidanceWeight   = 0.45f;

  // PUBLIC FUNCTIONS

  // PRIVATE FUNCTIONS


  void Start () {
    //TEMPORARY CHECKPOINTS.. TO BE REMOVED LATER
//    print("Boid SPAWNED\n");
    gameCTRL = FindObjectOfType<GameController>();
    enemies.Add(this);
    checkpoints = new Stack<Vector3>();
    checkpoints.Push(new Vector3( 8, 0, 0));
    checkpoints.Push(new Vector3( 2, 0,-4));
    checkpoints.Push(new Vector3( 0, 0, 2));
    checkpoints.Push(new Vector3(-4, 0, 4));
    checkpoints.Push(new Vector3(-8, 0, 0));

    rotation = Quaternion.LookRotation(checkpoints.Peek() - transform.position);
//    speedVector = transform.forward * currentSpeed;
//    gameCTRL.enemiesOnTheBoard++;
  }

  void Update () {
    if (checkpoints.Count > 1 && Vector3.Distance(transform.position, checkpoints.Peek()) < 0.5f) {  
      checkpoints.Pop();
    } 
    Vector3 lookAtPoint = (checkpoints.Peek() - transform.position).normalized + boidVector * boidWeight;;
    rotation = Quaternion.LookRotation(lookAtPoint);
    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * currentTurnSpeed);
//    transform.Translate(speedVector * Time.deltaTime);
    transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    if (checkpoints.Count == 1 && Vector3.Distance(transform.position, checkpoints.Peek()) < 0.1) {
      EnemyReachedTarget();
    }
  }   
}
