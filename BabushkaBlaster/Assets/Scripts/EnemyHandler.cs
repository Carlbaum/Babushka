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
//      boid.speedDiff       = new Vector3(0f,0f,0f);
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

            // cohesion
            // Lower HP => go to center of all other boids
//            if(lowestHP > other.currentHP) {
//              lowestHP = other.currentHP;
//            }
//            boid.cohesionWeight = boid.startingHP - boid.currentHP;
//            boid.boidWeight = boid.cohesionWeight/100.0f;
            if (boid.currentHP <= other.currentHP) {
              boid.averagePosition += relativePosition;
              cohesionCount++;
            } else if (boid.currentHP > other.currentHP && lowestHP > other.currentHP) { // Higher HP => go towards lower HP
              boid.averagePosition = other.transform.position;
              lowestHP = other.currentHP;
              cohesionCount = 1;
            }
//            } else { // equal HP
//              boid.averagePosition += other.transform.position;
//            }

            // avoidance
            if (relativeDistance < boid.avoidanceDistance) {
              boid.avoidanceVector += -relativePosition.normalized*(boid.avoidanceDistance-relativeDistance);
            }
            neighbourCount++;
          }
        }
      } 
      if (neighbourCount > 0) {
//        boid.speedDiff /= neighbourCount;
        boid.averagePosition /= cohesionCount;
        boid.avoidanceVector /= neighbourCount;
      }
    }
    foreach (Boid boid in enemies) {
//      Vector3 prevSpeedVector = boid.transform.forward * boid.currentSpeed;
      Vector3 speedVector = boid.averagePosition * boid.cohesionWeight +
//                            boid.speedDiff * boid.alignmentWeight +
                            boid.avoidanceVector * boid.avoidanceWeight;
//      boid.speedVector += speedVector;
//      if (boid.speedVector.magnitude > boid.maxSpeed) {
//        boid.speedVector = boid.speedVector.normalized * boid.maxSpeed;
//      }
//      speedVector
      boid.boidVector = speedVector.normalized;

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
