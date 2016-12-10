using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : Enemy {
  // PRIVATE VARIABLES
  public Stack<Vector3> checkpoints;

  //PUBLIC VARIABLES
  public Vector3 speedDiff;
  public Vector3 averagePosition;
  public Vector3 avoidanceVector;
  public Vector3 pathVector;

  public Vector3 boidVector;

  private GridHandlerNew gridHandler;
  private bool move = false;

  public float avoidanceDistance = 0.6f;
  public float cohesionDistance  = 1.2f;
//  public float boidWeight        = 0.0f; 
  public float pathWeight        = 1.0f; 
  public float alignmentWeight   = 0.1f;
  public float cohesionWeight    = 0.45f;
  public float avoidanceWeight   = 0.45f;

  // PUBLIC FUNCTIONS

  // PRIVATE FUNCTIONS


  void Start () {
    //TEMPORARY CHECKPOINTS.. TO BE REMOVED LATER
//    print("Boid SPAWNED\n");
    gameCTRL = FindObjectOfType<GameController>();
    gridHandler = FindObjectOfType<GridHandlerNew>();
    checkpoints = new Stack<Vector3>();
    /*checkpoints.Push(new Vector3( 8, 0, 0));
    checkpoints.Push(new Vector3( 7, 0, 0));
    checkpoints.Push(new Vector3( 6, 0, 0));
    checkpoints.Push(new Vector3( 5, 0,-1));
    checkpoints.Push(new Vector3( 4, 0,-2));
    checkpoints.Push(new Vector3( 3, 0,-3));
    checkpoints.Push(new Vector3( 2, 0,-4));
    checkpoints.Push(new Vector3( 2, 0,-3));
    checkpoints.Push(new Vector3( 2, 0,-2));
    checkpoints.Push(new Vector3( 1, 0,-1));
    checkpoints.Push(new Vector3( 0, 0, 0));
    checkpoints.Push(new Vector3( 0, 0, 1));
    checkpoints.Push(new Vector3( 0, 0, 2));
    checkpoints.Push(new Vector3(-1, 0, 2));
    checkpoints.Push(new Vector3(-2, 0, 3));
    checkpoints.Push(new Vector3(-3, 0, 3));
    checkpoints.Push(new Vector3(-4, 0, 4));
    checkpoints.Push(new Vector3(-5, 0, 3));
    checkpoints.Push(new Vector3(-6, 0, 2));
    checkpoints.Push(new Vector3(-7, 0, 1));
    checkpoints.Push(new Vector3(-8, 0, 0));
*/
    velocityVector = new Vector3(0,0,0);
    pathVector = new Vector3(0,0,0);

//    rotation = Quaternion.LookRotation(checkpoints.Peek() - transform.position);
//    speedVector = transform.forward * currentSpeed;
//    gameCTRL.enemiesOnTheBoard++;
    enemies.Add(this);
  }

  void Update () {
    if (move) {
      if (checkpoints.Count > 1 && Vector3.Distance(transform.position, checkpoints.Peek()) < 0.4f) {  
        checkpoints.Pop();
      } else if (checkpoints.Count == 1) {
        cohesionWeight = avoidanceWeight = alignmentWeight = 0;
      }

      float weightsSum = pathWeight + cohesionWeight + avoidanceWeight + alignmentWeight;
      pathWeight      /= weightsSum;
      cohesionWeight  /= weightsSum;
      avoidanceWeight /= weightsSum;
      alignmentWeight /= weightsSum;
  //    pathWeight = 1 - boidWeight;
  //    pathVector = (checkpoints.Peek() - transform.position).normalized * currentSpeed * pathWeight;
  //    boidVector += boidVector; // * currentSpeed * boidWeight ;
      // make sure that the pathVector contributes more than the boid behaviour
      /*if (boidVector.magnitude > pathVector.magnitude) {
        boidVector = boidVector.normalized * ;
      }*/
  //    if (boidVector.magnitude > boidWeight * currentSpeed) {
  ////      boidVector = boidVector.normalized * boidWeight * currentSpeed;
  //    }
      pathVector = checkpoints.Peek() - transform.position;
  //    velocityVector = (pathVector + boidVector);
      velocityVector += boidVector + pathVector.normalized * pathWeight;
      velocityVector.y = 0;
      if (velocityVector.magnitude > maxSpeed) {
        velocityVector = maxSpeed * velocityVector.normalized; 
      }
      //currentSpeed = velocityVector.magnitude;
  //    Vector3 lookAtPoint = checkpoints.Peek() - transform.position + boidVector * boidWeight;
  //    rotation = Quaternion.LookRotation(velocityVector);
  //    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * currentTurnSpeed);
      transform.Translate(velocityVector * Time.deltaTime);
  //    transform.Translate(transform.forward * velocityVector.magnitude * Time.deltaTime);
  //    transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
      if (checkpoints.Count == 1 && Vector3.Distance(transform.position, checkpoints.Peek()) < 0.2) {
        EnemyReachedTarget();
      }

    } else if (gridHandler.isPathFound) {
      move = true;
      //TODO FIX the checkpointPosition Stack.. right now the whole stack is being copied (and thus reversed) twice
      //      checkpointPosition = gridScript.getShortestPath();
      checkpoints = new Stack<Vector3>(gridHandler.getShortestPath());
//      rotation = Quaternion.LookRotation(checkpoints.Peek() - transform.position);

    }   
  }   
}
