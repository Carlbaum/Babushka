using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHandler : MonoBehaviour {

  enemyTypes enemyType;

  Vector3 spawnPoint  = new Vector3(-8, 0, 0);
  Vector3 targetPoint = new Vector3( 8, 0, 0);

  public Enemy[] EnemyType;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
   
//
//  private Vector3 CalcAvoidance(Vector3 relativePosition) {
//    
//  
//  }

  public void handleBoids(ref List<Enemy> enemies) {
    foreach (Boid boid in enemies) {
      int neighbourCount = 0;
      int cohesionCount = 0;
      float lowestHP = 9999999;
      boid.speedDiff       = new Vector3(0f,0f,0f);
      boid.averagePosition = new Vector3(0f,0f,0f);
      boid.avoidanceVector = new Vector3(0f,0f,0f);
      foreach (Boid other in enemies) {
        if (other != boid) {
          Vector3 relativePosition = (other.transform.position - 
            boid.transform.position);
          float relativeDistance = relativePosition.magnitude;
          if (relativeDistance < boid.cohesionDistance) {
//            // alignment
//            boid.speedDiff += other.transform.forward * other.currentSpeed - boid.transform.forward * boid.currentSpeed;
            boid.speedDiff += other.velocityVector - boid.velocityVector;

            // cohesion
            // Lower HP => go to center of all other boids
//            if(lowestHP > other.currentHP) {
//              lowestHP = other.currentHP;
//            }
//            boid.cohesionWeight = boid.startingHP - boid.currentHP;
//            boid.boidWeight = boid.cohesionWeight/100.0f;
//            if (boid.currentHP <= other.currentHP) {
//              boid.averagePosition += relativePosition;
//              cohesionCount++;
//            } else if (boid.currentHP > other.currentHP && lowestHP > other.currentHP) { // Higher HP => go towards lower HP
//              boid.averagePosition = other.transform.position;
//              lowestHP = other.currentHP;
//              cohesionCount = 1;
//            }
//            } else { // equal HP
//              boid.averagePosition += other.transform.position;
//            }


            // avoidance
            if (relativeDistance < boid.avoidanceDistance) {
            boid.avoidanceVector += -relativePosition.normalized * 2.0f * Mathf.Exp(-Mathf.Pow(boid.avoidanceDistance - relativeDistance,2.0f)/0.08f);

//              boid.avoidanceVector += -relativePosition.normalized * (boid.avoidanceDistance - relativeDistance);
              neighbourCount++;
            } else {
              boid.averagePosition += relativePosition;
              cohesionCount++;
            }
          }
        }
      } 
      if (neighbourCount > 0) {
        boid.speedDiff /= neighbourCount;
        boid.averagePosition /= cohesionCount;
        boid.avoidanceVector /= neighbourCount;
      }
    }
    foreach (Boid boid in enemies) {
//      boid.pathVector = boid.checkpoints.Peek() - boid.transform.position;
//      Vector3 prevSpeedVector = boid.transform.forward * boid.currentSpeed;
      Vector3 speedVector = (boid.averagePosition.normalized * boid.cohesionWeight) +
        (boid.speedDiff.normalized * boid.alignmentWeight) +
        (boid.avoidanceVector.normalized * boid.avoidanceWeight);
                            //(boid.pathVector      * boid.pathWeight);
//      boid.speedVector += speedVector;
//      if (boid.speedVector.magnitude > boid.maxSpeed) {
//        boid.speedVector = boid.speedVector.normalized * boid.maxSpeed;
//      }
//      speedVector
//      boid.boidVector = speedVector;
//      speedVector.y = 0;
      // TODO: TOWER CORRECTION

      boid.boidVector = speedVector;
    }
  }

  //TODO add enemyDeath to this class

  public void spawnEnemies(int numberOfEnemies, enemyTypes type, ref List<Enemy> enemies) {
      print("EnemyType: " + type);
      switch(type) {
        case enemyTypes.Chicken:
          for (int i = 0; i < numberOfEnemies; i++) {
            Enemy newEnemy = Instantiate(EnemyType[1], spawnPoint + new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)), Quaternion.identity) as Enemy;
            enemies.Add(newEnemy);
          }
          break;
        case enemyTypes.Protector:
          for (int i = 0; i < numberOfEnemies; i++) {
            Enemy newEnemy = Instantiate(EnemyType[0], spawnPoint + new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)), Quaternion.identity) as Enemy;
            enemies.Add(newEnemy);
          }
          break;
        default:
          break;
      }
  }
}
